using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace _TevLib.PolyNavMesh
{
    [CreateAssetMenu(fileName = "Baked data", menuName = "TevLib/PolyNavMesh/Baked data", order = 15)]
    public class NavMeshBakeDataSO : ScriptableObject
    {
        [field:SerializeField] public NavAgentDataSO AgentData { get; set; }
        public List<PolygonData> polygons = new();
        
        // 런타임 전용 --ID -> NavPolygon 빠른 조회
        private Dictionary<int, NavPolygon> _runtimeMap;
        
        private void OnEnable() => Init();
        public void Init()
        {
            if (_runtimeMap != null)
                return;
            
            BuildRuntimeMap();
        }


        /// <summary>
        /// 직렬화 데이터로부터 런타임 딕셔너리를 빌드
        /// NavMeshBaker가 베이킹을 완료한 뒤에도 호출 된다.
        /// </summary>
        public void BuildRuntimeMap()
        {
            _runtimeMap = new Dictionary<int, NavPolygon>(polygons.Count);

            foreach (PolygonData data in polygons)
            {
                _runtimeMap[data.id] = new NavPolygon
                {
                    Id = data.id,
                    Center = data.center,
                    Vertices = data.vertices,
                    Portals = data.portals
                };
                Debug.Log($"[NavMesh] Building runtime map with {polygons.Count} polygons");
            }
        }

        /// <summary>
        /// 넘겨진 월드 좌표를 포함하는 폴리곤을 반환한다.
        /// </summary>
        public bool GetPolygonAt(Vector2 worldPoint, out NavPolygon polygon)
        {
            foreach (NavPolygon p in _runtimeMap.Values)
            {
                if (ContainsPoint(p.Vertices, worldPoint))
                {
                    polygon = p;
                    return true;
                }
            }
            polygon = null;
            return false;
        }

        public bool TryGetPolygon(int id, out NavPolygon polygon)
        {
            polygon = null;
            return _runtimeMap != null && _runtimeMap.TryGetValue(id, out polygon);
        }

        public bool GetNearestPolygon(Vector2 worldPoint, out NavPolygon polygon)
        {
            polygon = null;
            if (_runtimeMap == null || _runtimeMap.Count == 0) return false;
            
            float bestSqr = float.MaxValue;
            foreach (NavPolygon p in _runtimeMap.Values)
            {
                float sqr = (p.Center - worldPoint).sqrMagnitude;
                if (sqr < bestSqr) {bestSqr = sqr; polygon = p;}
            }
            return polygon != null;
        }
        
        public void Clear() => polygons?.Clear();

        private static bool ContainsPoint(Vector2[] verts, Vector2 p)
        {
            for (int i = 0, j = verts.Length - 1; i < verts.Length; j = i++)
            {
                Vector2 a = verts[j], b = verts[i];
                // Cross(b-a, p-a) 음수면 p 가 엣지의 오른쪽(외부) - 외적 z 값만
                float cross = (b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x);
                if (cross < 0f) return false;
            }
            return true;
        }
    }
}