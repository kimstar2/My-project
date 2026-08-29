using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _TevLib.PolyNavMesh
{
    public class NavMeshBaker : MonoBehaviour
    {
        [SerializeField] private Tilemap groundMap;
        [SerializeField] private Tilemap obstacleMap;
        [SerializeField] private NavMeshBakeDataSO navMeshData;

        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private Color polygonColor = new Color(0.2f, 0.8f, 0.2f, 0.4f);
        [SerializeField] private Color portalColor = Color.yellow;
        [SerializeField] private Color centerColor = Color.green;

        private void OnEnable() => navMeshData?.Init();

        [ContextMenu("Bake NavMesh")]
        private void Bake()
        {
            Debug.Assert(groundMap != null,"groundMap is not assigned");
            Debug.Assert(obstacleMap != null, "obstacleMap is not assigned");
            Debug.Assert(navMeshData != null, "navMeshDataSO is not assigned");
            
            navMeshData.Clear();

            HashSet<Vector3Int> walkable = CollectWalkableCells();
            List<RectInt> rects = MergeIntoRectangles(walkable);
            rects = SplitRectsAtWallBoundaries(rects, walkable);
            BuildPolygons(rects, walkable);
            navMeshData.BuildRuntimeMap();
            
            Debug.Log($"[PolyNavMesh] Baked {navMeshData.polygons.Count} polygons from {walkable.Count} cells");
            SaveAsset();
        }

        private HashSet<Vector3Int> CollectWalkableCells()
        {
            HashSet<Vector3Int> walkable = new HashSet<Vector3Int>();
            groundMap.CompressBounds(); // 축소
            BoundsInt bounds = groundMap.cellBounds; // 가져옴
            
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cell = new Vector3Int(x, y, 0);
                if (groundMap.HasTile(cell) && !obstacleMap.HasTile(cell)) // 바닥있고 장애물없으면?
                    walkable.Add(cell); // 갈수있다!!!!
            }
            return walkable;
        }

        private List<RectInt> MergeIntoRectangles(HashSet<Vector3Int> walkable)
        {
            if (walkable.Count == 0) return new List<RectInt>();
            
            int yMin = int.MaxValue, yMax = int.MinValue;
            int xMin = int.MaxValue, xMax = int.MinValue;
            foreach (Vector3Int cell in walkable)
            {
                if (cell.y < yMin) yMin = cell.y;
                if (cell.y > yMax) yMax = cell.y;
                if (cell.x < xMin) xMin = cell.x;
                if (cell.x > xMax) xMax = cell.x; // 셀들의 각 하한 상한을 뽑아냄
            }
            
            List<RectInt> rects = new List<RectInt>();
            // 현재 확장 중인 사각형 : key = (xMin, xMax exclusive), value = 시작 y
            Dictionary<(int,int),int> active =  new Dictionary<(int,int), int>();

            for (int y = yMin; y <= yMax + 1; y++) // 마지막으로 돌던 사각형도 rect 집합에 들어가게 하기 위해 +1 까지
            {
                // 현재 행의 연속된 walkable 구간을 구한다 (가로로 쭉 돌면서 블럭들을 구함)
                var currentSegs = new HashSet<(int start, int end)>();
                if (y <= yMax)
                {
                    int? segStart = null;
                    for (int x = xMin; x <= xMax + 1; x++)
                    {
                        bool isWalkable = x <= xMax && walkable.Contains(new Vector3Int(x, y, 0));
                        if (isWalkable && segStart == null) segStart = x; // 걸을 수 있는 셀
                        // 시작이라면 x를 segment 시작으로
                        else if (!isWalkable && segStart != null)
                        {
                            currentSegs.Add((segStart.Value, x)); 
                            segStart = null;
                        } 
                        // 걸을 수 없고 시작한 상태라면 현재 세그먼트를 시작과 끝으로 해서 currentSegs에 저장     
                    }
                }
                
                var newActive = new Dictionary<(int start,int end), int>();
                foreach ((int start, int end) seg in currentSegs)
                {
                    // 완전 일치하는 사각형이 있다면 해당 사각형의 y 값을 가져오고 그렇지 않다면 현재 y 값을 넣는다.
                    newActive[seg] = active.GetValueOrDefault(seg, y);
                }
                
                // active중에서 현재 currentSeg 에 속하지 못한것들은 이번행에서 끊긴 사각형이다.
                // 따라서 사각형으로 정리

                foreach (KeyValuePair<(int, int), int> kv in active)
                {
                    if (!currentSegs.Contains(kv.Key))
                        rects.Add(new RectInt(kv.Key.Item1, kv.Value,kv.Key.Item2-kv.Key.Item1,y-kv.Value));
                }
                active = newActive;
            }
            return rects;
        }
        
        private List<RectInt> SplitRectsAtWallBoundaries(List<RectInt> rects, HashSet<Vector3Int> walkable)
        {
            bool changed = true;
            while (changed)
            {
                changed = false;
                var next = new List<RectInt>();
                foreach (RectInt rect in rects)
                {
                    List<RectInt> split = TrySplitRect(rect, walkable);
                    if (split != null)
                    {
                        next.AddRange(split);
                        changed = true;
                    }
                    else
                        next.Add(rect);
                }
                rects = next;
            }
            return rects;
        }

        private List<RectInt> TrySplitRect(RectInt rect, HashSet<Vector3Int> walkable)
        {
            // 상/하 변 : x 축 분할
            int? splitX = FindHorizontalSideSplit(rect, -1, walkable)
                          ?? FindHorizontalSideSplit(rect, +1, walkable);

            if (splitX.HasValue)
            {
                return new List<RectInt>
                {
                    new RectInt(rect.xMin, rect.yMin, splitX.Value - rect.xMin, rect.height),
                    new RectInt(splitX.Value, rect.yMin, rect.xMax - splitX.Value, rect.height)
                };
            }
            
            // 좌/우 변 : y 축 분할
            int? splitY = FindVerticalSplitSide(rect, -1, walkable) ??
                          FindVerticalSplitSide(rect, +1, walkable);
            if (splitY.HasValue)
            {
                return new List<RectInt>
                {
                    new RectInt(rect.xMin, rect.yMin, rect.width, splitY.Value - rect.yMin),
                    new RectInt(rect.xMin, splitY.Value, rect.width, rect.yMax - splitY.Value)
                };
            }

            return null;
        }

        public struct WorldRect
        {
            public float XMin, YMin, XMax, YMax;
        }
        
        /// <summary>
        /// 주어진 Rect들을 축소 시킨 후 폴리곤으로 변경한다.
        /// </summary>
        private void BuildPolygons(List<RectInt> rects, HashSet<Vector3Int> walkable)
        {
            float radius = navMeshData.AgentData != null
                ? navMeshData.AgentData.AgentRadius
                : 0f;
            
            // 벽과 맞닿은 변을 축소한 월드 좌표 사각형 (인덱스는 rects와 1:1 대응)
            WorldRect[] shrink = new WorldRect[rects.Count];
            for (int i = 0 ; i < rects.Count; i++)
                shrink[i] = ShrinkRect(rects[i], walkable,radius);
            
            // 축소된 직사각형 -> PolygonData
            for (int i = 0; i < rects.Count; i++)
                navMeshData.polygons.Add(RectToPolygon(i , shrink[i]));
            
            // 인접 쌍 검사 -> Portal 양방향 연결
            // 인접 여부는 원본 셀 좌표로 판별하고, 끝점 좌표는 축소본을 사용한다.
            for (int i = 0; i < rects.Count; i++)
            for (int j = i + 1; j < rects.Count; j++)
                TryAddPortal(rects[i], rects[j], shrink[i], shrink[j],
                    navMeshData.polygons[i], navMeshData.polygons[j]);
        }

        /// <summary>
        /// 사각형의 4개 변 중 '완전히 벽과 맞닿은 변' 만 radius만큼 안쪽으로 민다.
        /// 한 칸이라도 walkable 이웃이 있는 변(=포털이 생기는 변)은 그대로 둔다.
        /// </summary>
        private WorldRect ShrinkRect(RectInt rect, HashSet<Vector3Int> walkable, float radius)
        {
            Vector2 bl = CellCornerToWorld(groundMap,rect.xMin,rect.yMin);
            Vector2 tr = CellCornerToWorld(groundMap,rect.xMax,rect.yMax);
            // rect의 xMax와 yMax는 exclusive힘 해당 좌표의 좌하단을 구하면 실질적 영역인 xMax-1,yMax-1의 우상단과 같음

            float left = IsSideWall(rect, -1, 0, walkable) ? radius : 0f; // 왼쪽변
            float right = IsSideWall(rect, +1, 0, walkable) ? radius : 0f; // 오른쪽변
            float bottom = IsSideWall(rect, 0, -1, walkable) ? radius : 0f; // 아래변
            float top = IsSideWall(rect, 0, +1, walkable) ? radius : 0f; // 위변
            
            // 마주보면 두변의 축소량 합이 폭/높이를 넘으면 폴리곤이 뒤집힌다. -> clamp
            ClampInset(ref left, ref right, tr.x - bl.x);
            ClampInset(ref bottom, ref top , tr.y - bl.y);

            return new WorldRect
            {
                XMin = bl.x + left,
                YMin = bl.y + bottom,
                XMax = tr.x - right,
                YMax = tr.y - top
            };
        }
        
        

        private void SaveAsset()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(navMeshData);
            AssetDatabase.SaveAssets();
#endif
        }
        
        #region static Helper method
        
        /// <summary>
        /// 셀 격자 좌표 (정수 모서리)를 월드 좌표로 변환한다.
        /// CellToWorld는 셀 중심이 아닌 셀 경계를 기준으로 한다.
        /// </summary>
        private static Vector2 CellCornerToWorld(Tilemap groundMap, int x , int y)
        => groundMap.CellToWorld(new Vector3Int(x,y,0));

        private static bool IsSideWall(RectInt rect, int dx, int dy, HashSet<Vector3Int> walkable)
        {
            if (dx != 0)
            {
                int x = dx < 0 ? rect.xMin - 1 : rect.xMax; // xMax 는 exclusive -> 바깥 첫 칸
                for (int y = rect.yMin; y < rect.yMax; y++)
                    if (walkable.Contains(new Vector3Int(x, y, 0))) return false;
            }
            else
            {
                int y = dy < 0 ? rect.yMin - 1 : rect.yMax;
                for (int x = rect.xMin; x < rect.xMax; x++)
                    if (walkable.Contains(new Vector3Int(x, y, 0))) return false;
            }
            return true;
        }
        
        // 좁은 통로 보호 : 마주보는 변의 축소량 합(a+b)이 extent를 넘지 않게 비례 축소한다.
        // 최소 두께 MinExtent는 남겨 폴리곤/포털이 완전히 사라지지 않도록 한다.
        private const float MinExtent = 0.05f;
        private static void ClampInset(ref float a, ref float b, float extent)
        {
            float max = extent - MinExtent;
            if (a + b > max && a + b > 0f)
            {
                float scale = max / (a + b);
                a *= scale;
                b *= scale;
            }
        }
        
        
        private PolygonData RectToPolygon(int id, WorldRect worldRect)
        {
            // 꼭짓점을 CCW(반시계 방향)로 정렬 -- ContainsPoint 판별에 필요
            Vector2 bl = new Vector2(worldRect.XMin, worldRect.YMin);
            Vector2 br = new Vector2(worldRect.XMax, worldRect.YMin);
            Vector2 tr = new Vector2(worldRect.XMax, worldRect.YMax);
            Vector2 tl = new Vector2(worldRect.XMin, worldRect.YMax);
            
            return new PolygonData
            {
                id = id,
                center = (bl + br + tr + tl) * 0.25f,
                vertices = new[] {bl, br, tr, tl}, // CCW
                portals = new List<PortalData>()
            };
        }
        
        /// <summary>
        /// 두 직사각형이 엣지를 공유하면 Portal을 양방향으로 추가한다
        /// 공유 변은 벽이 아니므로 축소되지 않아 양쪽 좌표가 정확히 일치한다.
        /// 포털 끈점(수직 방향 범위)은 축소된 사각형 기준으로 계산되어 벽에서 떨어진다.
        /// </summary>
        private void TryAddPortal(RectInt rectA, RectInt rectB, WorldRect wRectA, WorldRect wRectB,
            PolygonData polyA, PolygonData polyB)
        {
            const float eps = 1e-4f; // 입실론
            
            // 수평 인접 : 공유 세로 엣지 -> 포털을 y축 방향 세그먼트
            if (rectA.xMax == rectB.xMin || rectB.xMax == rectA.xMin)
            {
                // 공유 변 (= 한쪽의 오른쪽변)은 축소되지 않았으므로 그 x 를 그대로 쓴다
                float px = (rectA.xMax == rectB.xMin) ? wRectA.XMax : wRectB.XMax;
                float yMin = Mathf.Max(wRectA.YMin, wRectB.YMin); // 2개의 min 중 큰것
                float yMax = Mathf.Min(wRectA.YMax, wRectB.YMax); // 2개의 max 중 작은것으로 교집을 찾음
                if (yMax - yMin <= eps) return;

                Vector2 pA = new Vector2(px, yMin);
                Vector2 pB = new Vector2(px, yMax);
                polyA.portals.Add(new PortalData {PointA = pA, PointB = pB, neighborId = polyB.id});
                polyB.portals.Add(new PortalData {PointA = pA, PointB = pB, neighborId = polyA.id});
            } 
            // 수직 인접 :공유 가로 엣지 -> 포털은 X 방향 세그먼트
            else if (rectA.yMax == rectB.yMin || rectB.yMax == rectA.yMin)
            {
                float py = (rectA.yMax == rectB.yMin) ? wRectA.YMax : wRectB.YMax;
                float xMin = Mathf.Max(wRectA.XMin, wRectB.XMin);
                float xMax = Mathf.Min(wRectA.XMax, wRectB.XMax);
                if (xMax - xMin <= eps) return;

                Vector2 pA = new Vector2(xMin, py);
                Vector2 pB = new Vector2(xMax, py);
                polyA.portals.Add(new PortalData {PointA =  pA, PointB = pB, neighborId = polyB.id});
                polyB.portals.Add(new PortalData {PointA =  pA, PointB = pB, neighborId = polyA.id});
            }
        }
        
        /// <summary>
        /// 수평 변(하: dy =-1 , 상: dy =+1)을 x 방향으로 스캔하여
        /// 벽이 끊어지는 전환이 발생하는 x 좌표를 반환한다.
        /// </summary>
        private static int? FindHorizontalSideSplit(RectInt rect, int dy, HashSet<Vector3Int> walkable)
        {
            if (rect.width <= 1) return null;
            int checkY = dy < 0 ? rect.yMin -1 : rect.yMax; // 사각형의 y축 아래 또는 y축 위를 저장.
            
            bool prevWall = !walkable.Contains(new Vector3Int(rect.xMin,checkY,0));
            for (int x = rect.xMin + 1; x < rect.xMax; x++)
            {
                bool wall = !walkable.Contains(new Vector3Int(x, checkY,0));
                if (wall != prevWall) return x;
                prevWall = wall;
            }
            return null;
        }

        private static int? FindVerticalSplitSide(RectInt rect, int dx, HashSet<Vector3Int> walkable)
        {
            if (rect.height <= 1) return null;
            int checkX = dx < 0 ? rect.xMin -1 : rect.xMax ;
            
            bool prevWall = !walkable.Contains(new Vector3Int(checkX,rect.yMin,0));
            for (int y = rect.yMin + 1; y < rect.yMax; y++)
            {
                bool wall = !walkable.Contains(new Vector3Int(checkX,y,0));
                if (wall != prevWall) return y;
                prevWall = wall;
            }
            return null;
        }
        #endregion
        
        #if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos || navMeshData == null) return;

            foreach (PolygonData poly in navMeshData.polygons)
            {
                // 폴리곤 윤곽선
                Gizmos.color = polygonColor;
                Handles.color = polygonColor;
                DrawPolygonGizmo(poly.vertices);
                
                // 중심점
                Gizmos.color = centerColor;
                Gizmos.DrawWireSphere(poly.center, 0.1f);
                
                // Portal 엣지
                Gizmos.color = portalColor;
                foreach (PortalData portal in poly.portals)
                {
                    Gizmos.DrawLine(portal.PointA, portal.PointB);
                    Gizmos.DrawWireSphere((portal.PointA + portal.PointB) * 0.5f, 0.2f);
                }
            }
        }

        private static void DrawPolygonGizmo(Vector2[] verts)
        {
            for (int i = 0; i < verts.Length; i++)
            {
                Handles.DrawLine(verts[i], verts[(i + 1) % verts.Length], 4f);
            }
        }

#endif
    }
}