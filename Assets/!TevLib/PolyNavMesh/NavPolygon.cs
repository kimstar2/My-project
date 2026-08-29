using System.Collections.Generic;
using UnityEngine;

namespace _TevLib.PolyNavMesh
{
    /// <summary>
    /// 런타임에서 사용하는 폴리곤의 정적 데이터
    /// </summary>
    public class NavPolygon
    {
        public int Id;
        public Vector2 Center;
        public Vector2[] Vertices;
        public List<PortalData> Portals;

        public override bool Equals(object obj) => obj is NavPolygon p && p.Id == Id;
        public override int GetHashCode() => Id;

        public static bool operator==(NavPolygon a, NavPolygon b)
        {
            if (a is null) return b is null;
            return a.Equals(b);
        }
        public static bool operator!=(NavPolygon a, NavPolygon b) => !(a == b);
    }
}