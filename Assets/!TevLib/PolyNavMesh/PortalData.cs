using System;
using UnityEngine;

namespace _TevLib.PolyNavMesh
{
    /// <summary>
    /// 두 폴리곤이 공유하는 엣지(Portal). Funnel 알고리즘에서 경로가 통과하는 "문"에 해당.
    ///
    /// pointA / pointB 는 월드좌표계 기준 두 끝점이며,
    /// left / right 방향은 경로 진행 방향에 따라 런타임에 결정
    /// (NavMesh에서는 이를 "Portal Edge" 라고 부름)
    /// </summary>
    [Serializable]
    public struct PortalData
    {
        public Vector2 PointA;
        public Vector2 PointB;
        public int neighborId; // 이 포탈 너머의 폴리곤 ID
    }
}
//Funnel 알고리즘은 스트링 풀링 알고리즘의 정식 명칭-