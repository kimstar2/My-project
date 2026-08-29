using System;
using System.Collections.Generic;
using UnityEngine;

namespace _TevLib.PolyNavMesh
{
    /// <summary>
    /// NavMesh 를 구성하는 하나의 블록 폴리곤 (직렬화용)
    /// Unity NavMesh에서 Navigation Polygon(삼각형 메시의 한 면)에 해당한다
    /// </summary>
    [Serializable]
    public class PolygonData
    {
        public int id;
        public Vector2 center; // 폴리곤 중심점 (월드 좌표) -- a* 휴리스틱 계산에 사용
        public Vector2[] vertices; // 꼭짓점 배열, CCW-Counter clock wise (시계방향) 순서 (직사각형이면 4개)
        public List<PortalData> portals = new(); // 인접 폴리곤 연결 목록
    }
}