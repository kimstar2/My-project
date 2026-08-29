using System.Collections.Generic;
using UnityEngine;

namespace _TevLib.PolyNavMesh
{
    public static class Funnel
    {
        public static List<Vector2> StringPull(
            Vector2 start,
            Vector2 end,
            List<(Vector2 left, Vector2 right)> portals)
        {
            var path = new List<Vector2> { start };

            if (portals.Count == 0)
            {
                path.Add(end);
                return path;
            }

            var pts = new List<(Vector2 left, Vector2 right)>(portals.Count + 2);
            pts.Add((start, start));
            pts.AddRange(portals);
            pts.Add((end, end));

            Vector2 apex = start;
            Vector2 portalLeft = start;
            Vector2 portalRight = start;
            int apexIdx = 0; // 최고 idx
            int leftIdx = 0;
            int rightIdx = 0;

            for (int i = 1; i < pts.Count; i++)
            {
                (Vector2 newLeft , Vector2 newRight) = pts[i];
                
                // 오른쪽 경계 갱신
                if (TriArea2(apex, portalRight, newRight) >= 0f)
                {
                    if (apex == portalRight || TriArea2(apex, portalLeft, newRight) < 0f)
                    {
                        portalRight = newRight;
                        rightIdx = i;
                    }
                    else
                    {
                        if (portalLeft != apex && portalLeft != end)
                            path.Add(portalLeft);

                        apex = portalLeft;
                        apexIdx = leftIdx;
                        portalLeft = apex;
                        portalRight = apex;
                        leftIdx = apexIdx;
                        rightIdx = apexIdx;
                        i = apexIdx;
                        continue;
                    }
                }
                if (TriArea2(apex, portalLeft, newLeft) <= 0f)
                {
                    if (apex == portalLeft || TriArea2(apex, portalRight, newLeft) > 0f)
                    {
                        portalLeft = newLeft;
                        leftIdx = i;
                    }
                    else
                    {
                        if (portalRight != apex && portalRight != end)
                            path.Add(portalRight);

                        apex = portalRight;
                        apexIdx = rightIdx;
                        portalLeft = apex;
                        portalRight = apex;
                        leftIdx = apexIdx;
                        rightIdx = apexIdx;
                        i = apexIdx;
                        continue;
                    }
                }
            }
            if (path.Count == 0 || path[^1] != end)
                path.Add(end);
            
            return path; // 내일 할거임
        }

        /// <summary>
        /// 삼각형 (a, b, c)의 부호 있는 넓이 × 2 (2D Cross Product).
        /// 양수: c가 벡터 a→b 의 왼쪽에 있음 (CCW)
        /// 음수: c가 벡터 a→b 의 오른쪽에 있음 (CW)
        /// 0   : a, b, c가 일직선
        /// </summary>
        private static float TriArea2(Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 baV = b - a;
            Vector2 caV = c - a;
            float cross = baV.x * caV.y - baV.y * caV.x;
            return cross;
        }

    }
}