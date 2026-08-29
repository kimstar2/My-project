using UnityEngine;

namespace _TevLib.CustomUtility
{
    public static class CtGizmoUtils
    {
        public static void DrawArrowGizmo(Vector3 start, Vector3 end)
        {
            Vector3 dir = end - start;
            
            Vector3 normalDir = dir.normalized;
            Vector3 arrowStart = end - normalDir * 0.25f;
            Vector3 arrowEnd = end - normalDir * 0.15f;
            const float arrowSize = 0.05f;
            
            Vector3 triangleA = arrowStart + (Quaternion.Euler(0,0,-90f) * normalDir) * arrowSize;
            Vector3 triangleB = arrowStart + (Quaternion.Euler(0,0,90f) * normalDir) * arrowSize;
            
            Gizmos.DrawLine(start, end);
            Gizmos.DrawLine(triangleA, arrowEnd);
            Gizmos.DrawLine(triangleB, arrowEnd);
            Gizmos.DrawLine(triangleA, triangleB);
        }
    }
}