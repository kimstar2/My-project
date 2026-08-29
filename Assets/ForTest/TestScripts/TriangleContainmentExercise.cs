using UnityEngine;

namespace _02.Scenes.ForTest.TestScripts
{
    public class TriangleContainmentExercise : MonoBehaviour
    {
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        [SerializeField] private Transform pointC;
        [SerializeField] private Transform testPoint;

        [SerializeField] private float epsilon = 0.001f;

        private void OnDrawGizmos()
        {
            if (pointA == null ||
                pointB == null ||
                pointC == null ||
                testPoint == null)
            {
                return;
            }

            Vector2 a = pointA.position;
            Vector2 b = pointB.position;
            Vector2 c = pointC.position;
            Vector2 p = testPoint.position;

            bool isInside = IsInsideTriangle(a, b, c, p);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawLine(pointB.position, pointC.position);
            Gizmos.DrawLine(pointC.position, pointA.position);

            Gizmos.color = isInside ? Color.green : Color.red;
            Gizmos.DrawSphere(testPoint.position, 0.15f);
        }

        private bool IsInsideTriangle(
            Vector2 a,
            Vector2 b,
            Vector2 c,
            Vector2 p)
        {
            
            
            // TODO 1: A → B 기준으로 P의 외적
            float crossAB = Cross(a,b,p);

            // TODO 2: B → C 기준으로 P의 외적
            float crossBC =  Cross(b,c,p);

            // TODO 3: C → A 기준으로 P의 외적
            float crossCA =  Cross(c,a,p);

            if (crossAB < -epsilon)
                return false;
            if (crossBC < -epsilon)
                return false;
            if (crossCA < -epsilon)
                return false;
            
            return true;
        }

        private static float Cross(
            Vector2 start,
            Vector2 end,
            Vector2 point)
        {
            Vector2 s2e = end - start;
            Vector2 s2p = point - start;
            
            float cross = s2e.x * s2p.y - s2e.y * s2p.x;
            return cross;
        }
    }
}