using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _TevLib.CoreLib;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _TevLib.PolyNavMesh
{
    public class NavAgent2D : MonoBehaviour
    {
        [SerializeField] private NavMeshBakeDataSO navMeshData;
        [SerializeField] private float speed;
        
        private CancellationTokenSource _cts = new();
        private bool _isCalculating;
        private Vector2 _lastDestination;

        private readonly Vector2[] _waypoints = new Vector2[256];
        private int _waypointCount;
        private int _waypointIndex;

        public bool PathPending => _isCalculating;
        public bool HasPath { get; private set; }
        public bool IsPathStale { get; private set; }
        public bool IsMoving { get; private set; }
        
        public bool IsPartialPath { get; private set; }
        
        public float Speed
        {
            get => speed;
            set => speed = Mathf.Max(0f, value);
        }

        #region Compute Path

        // Phase 1 : A* on Polygons
        // 탐색을 '셀'이 아닌 '폴리곤'으로 진행 한다. G값 계산에 폴리곤 중심 간 거리를 사용하고,
        // H값(휴리스틱)도 목표 폴리곤 중심까지의 유클리드 거리를 사용한다.
        // isPartial=true: 목표에 도달하지 못했으나 최근접 노드까지의 경로를 반환
        private (List<NavPolygon> path, bool isPartial) AStarPolygons(NavPolygon start,
            NavPolygon end, CancellationToken ct)
        {
            var startNode = new AStarNode(start) {G = 0,F = CalcH(start, end)};
            
            var openList = new MinHeap<AStarNode>((a, b) => a.F.CompareTo(b.F));
            var closedSet = new HashSet<int>();
            
            openList.Push(startNode);
            
            // 목표에 가장 가까이 도달한 노드를 추적 (부분 경로용)
            AStarNode bestNode = startNode;
            float bestH = CalcH(start, end);

            while (openList.Count > 0)
            {
                if (ct.IsCancellationRequested) return (null, false);
                
                AStarNode current = openList.Pop();
                closedSet.Add(current.Polygon.Id);

                if (current.Polygon == end)
                    return (ReconstructPolyPath(current), false);
                
                float h = CalcH(current.Polygon, end);
                if (h < bestH) {bestH = h; bestNode = current;}

                foreach (PortalData portal in current.Polygon.Portals)
                {
                    if (closedSet.Contains(portal.neighborId)) continue;
                    if (!navMeshData.TryGetPolygon(portal.neighborId, out NavPolygon neighbor))
                        continue;
                    float newG = current.G + Vector2.Distance(current.Polygon.Center, neighbor.Center);
                    
                    AStarNode existing = openList.Find(n => n.Polygon.Id == portal.neighborId);
                    if (existing != null)
                    {
                        // Decrease-Key: 더 짧은 경로 발견 시 힙 순서 복구
                        if (newG < existing.G)
                        {
                            existing.G = newG;
                            existing.F = newG + CalcH(neighbor, end);
                            existing.Parent = current;
                            openList.DecreaseKey(existing);
                        }
                    }
                    else
                    {
                        openList.Push(new AStarNode(neighbor)
                        {
                            G = newG,
                            F = newG + CalcH(neighbor, end),
                            Parent = current,
                        });
                    }
                }
            }
            // 오픈 리스트 소진 : 목표 불가 : bestNode까지의 부분 경로 반환
            return (ReconstructPolyPath(bestNode), true); 
        }

        private static List<NavPolygon> ReconstructPolyPath(AStarNode end)
        {
            var path = new List<NavPolygon>();
            for (AStarNode n = end; n != null; n = n.Parent)
                path.Add(n.Polygon);
            path.Reverse();
            return path; // 폴리곤 경로를 역으로 배열
        }
        
        private static float CalcH(NavPolygon a, NavPolygon b) => Vector2.Distance(a.Center, b.Center);
        
        #endregion 
        
        // Phase 2: Portal 추출
        // 폴리곤 시퀀스에서 각 인접 폴리곤 쌍의 공유 Portal을 찾고,
        // 이동 방향(from.center → to.center) 기준으로 left/right를 결정한다.
        // 결정 방법: 방향 벡터 dir와 포털 중점 mid를 기준으로
        // Cross(dir, pointA - mid) > 0 이면 pointA가 왼쪽.
        // (2D Cross Product: dir.x*(p.y-mid.y) - dir.y*(p.x-mid.x))
        private static List<(Vector2 left, Vector2 right)> ExtractOrientedPortal(List<NavPolygon> polyPath)
        {
            var portals = new List<(Vector2 left, Vector2 right)>(polyPath.Count - 1);

            for (int i = 0; i < polyPath.Count - 1; i++)
            {
                NavPolygon from = polyPath[i];
                NavPolygon to = polyPath[i + 1];
                PortalData? found = null;
                foreach (PortalData p in from.Portals)
                {
                    if (p.neighborId == to.Id) {found = p; break; }
                }
                if (found == null) continue;

                Vector2 pA = found.Value.PointA;
                Vector2 pB = found.Value.PointB;
                Vector2 dir = to.Center - from.Center;
                Vector2 mid = (pA + pB) * 0.5f;
                
                // Cross > 0 이면 pA가 이동 방향의 왼쪽 중심에서 pa로 가는 벡터와 방향간의 외적의 z 값만 가져왔다.
                // (A_y * B_z - A_z * B_y, A_z * B_x - A_x * B_z, A_x * B_y - A_y * B_x)

                float cross = dir.x * (pA.y - mid.y) - dir.y * (pA.x - mid.x);
                portals.Add(cross >= 0f ? (pA,pB) : (pB,pA) );
            }
            return portals;
        }

        public async UniTask<int> GetPath(Vector2 start, Vector2 destination, Vector2[] pointArr)
        {
            if (_isCalculating)
                _cts?.Cancel();
            
            if (HasPath && (destination - _lastDestination).sqrMagnitude > 0.001f)
                IsPathStale = true;

            if (_cts is null or { IsCancellationRequested: true })
                _cts = new CancellationTokenSource();

            CancellationToken token = _cts.Token;
            
            try
            {
                _isCalculating = true;

                // WebGL 싱글 스레드 환경이라 Task.Run 사용불가
                // Task.FromResult로 동기 실행하되 , asynk Task 인터페이스는 유지
#if UNITY_WEBGL
                pathTask = Task.FromResult(CalculatePath(start, destination, token));
#else
                Task<(List<Vector2>, PathStatus)> pathTask = Task.Run(() => CalculatePath(start, destination, token), token);
#endif
                (List<Vector2> waypoints, PathStatus status) = await pathTask;

                int count = 0;
                if (status != PathStatus.Invalid)
                {
                    for (int i = 0; i < waypoints.Count && i < pointArr.Length; i++, count++)
                        pointArr[i] = waypoints[i];

                    HasPath = true;
                    IsPathStale = false;
                    IsPartialPath = status == PathStatus.Partial;
                    _lastDestination = destination;
                }
                else
                {
                    HasPath = false;
                    IsPathStale = false;
                    IsPartialPath = false;
                }

                return status != PathStatus.Invalid ? count : -1;
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                HasPath = false;
                IsPathStale = false;
                return -1;
            }
            finally
            {
                _isCalculating = false;
            }
        }
        private enum PathStatus { Complete , Partial , Invalid }

        private (List<Vector2> path, PathStatus status) CalculatePath(Vector2 start, Vector2 end, CancellationToken ct)
        {
            // 출발점이 폴리곤 바깥 (radius 경계선 위 등)이면 가장 가까운 폴리곤의 최근접 점으로 보정
            bool startOutside = !navMeshData.GetPolygonAt(start, out NavPolygon startPoly);
            if (startOutside)
            {
                if (!navMeshData.GetNearestPolygon(start, out startPoly))
                    return (null, PathStatus.Invalid);
            }
            bool destOutside = !navMeshData.GetPolygonAt(end , out NavPolygon endPoly);
            if (destOutside)
            {
                if (!navMeshData.GetNearestPolygon(end, out endPoly))
                    return (null, PathStatus.Invalid);
            }
            
            // 같은 폴리곤이면 직선으로 이동 가능
            if (startPoly == endPoly)
            {
                Vector2 effectiveEnd = destOutside
                    ? ClosestPointOnPolygon(endPoly.Vertices, end)
                    : end;
                PathStatus samePolyStatus = destOutside ? PathStatus.Partial : PathStatus.Complete;
                return (new List<Vector2> {start, effectiveEnd}, samePolyStatus);
            }
            
            // Phase 1: A* — 폴리곤 시퀀스 탐색 (도달 불가 시 최근접 폴리곤까지 부분 경로 반환)
            (List<NavPolygon> polyPath, bool isPartialAStar) = AStarPolygons(startPoly, endPoly, ct);
            if (polyPath == null) return (null, PathStatus.Invalid);
            
            // Phase 2 : Portal 추출
            List<(Vector2, Vector2)> portals = ExtractOrientedPortal(polyPath);
            
            // 부분 경로 : 마지막 폴리곤에서 목적지에 가까운 점을 종착점으로 사용
            bool isPartial = isPartialAStar || destOutside;
            Vector2 funnelEnd = isPartial
                ? ClosestPointOnPolygon(polyPath[^1].Vertices, end)
                : end;
            
            // Phase 3 : Funnel -- 실제 Waypoint 계산
            // 벽 이격은 NavMeshBaker 가 실제 폴리곤/포털을 축소(ShrinkRect)할 때 이미 적용 했으므로
            // Funnel은 포털 끝점을 그대로 잇는 순수 string-pull만 수행한다.
            List<Vector2> waypoints = Funnel.StringPull(start, funnelEnd, portals);
            return (waypoints, isPartial ? PathStatus.Partial : PathStatus.Complete);
        }


        private static Vector2 ClosestPointOnPolygon(Vector2[] verts, Vector2 point)
        {
            // 내부 판별 : 모든 엣지의 왼쪽이면 내부
            bool inside = true;
            for (int i = 0, j = verts.Length - 1; i < verts.Length; j = i++)
            {
                float cross = (verts[i].x - verts[j].x) * (point.y - verts[j].y) 
                    - (verts[i].y - verts[j].y) * (point.x - verts[j].x);
                if (cross < 0f)
                {
                    inside = false;
                    break;
                }
            }

            if (inside) return point;
            
            // 각 변 위의 최근접 점 중 가장 가까운 것을 선택
            float bestSqr = float.MaxValue;
            Vector2 best = verts[0];
            for (int i = 0, j = verts.Length - 1; i < verts.Length; j = i++)
            {
                Vector2 closest = ClosestPointOnSegment(verts[j], verts[i], point);
                float sqr = (closest - point).sqrMagnitude;
                if (sqr < bestSqr) {bestSqr = sqr; best = closest;}
            }
            return best;
        }

        private static Vector2 ClosestPointOnSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 ab = b - a;
            float t = Vector2.Dot(p-a,ab) / Vector2.Dot(ab, ab);
            t = Mathf.Clamp01(t);
            return a + ab * t;
        }

        public void InvalidatePath()
        {
            if (HasPath) IsPathStale = true;
        }

        /// <summary>
        /// 목적지를 설정하고 경로를 계산한뒤 AgentSpeed로 이동을 시작
        /// </summary>
        public async void SetDestination(Vector2 destination)
        {
            ResetPath();
            
            int count = await GetPath((Vector2)transform.position, destination, _waypoints);
            if (count <= 1) return;

            _waypointCount = count;
            _waypointIndex = 1; // 0은 인덱스 출발 지점
            IsMoving = true;
        }

        public void ResetPath()
        {
            if (_isCalculating) _cts?.Cancel();
            IsMoving = false;
            _waypointCount = 0;
            _waypointIndex = 0;
            HasPath = false;
            IsPathStale = false;
            IsPartialPath = false;
        }

        private void Update()
        {
            if (!IsMoving || _waypointIndex >= _waypointCount) return;

            Vector2 target = _waypoints[_waypointIndex];
            Vector2 current = transform.position;
            
            Vector2 next = Vector2.MoveTowards(current, target, Speed * Time.deltaTime);
            transform.position = next;

            if ((next - target).sqrMagnitude <= 0.0001f)
            {
                _waypointIndex++;
                if (_waypointIndex >= _waypointCount)
                    IsMoving = false;
            }
        }
    }
}