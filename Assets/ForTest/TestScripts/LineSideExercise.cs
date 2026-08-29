using UnityEngine;

namespace _02.Scenes.ForTest.TestScripts
{
    public class LineSideExercise : MonoBehaviour
    {
        [SerializeField] private Transform lineStart;
        [SerializeField] private Transform lineEnd;
        [SerializeField] private Transform testPoint;

        [SerializeField] private float epsilon = 0.001f;

        private void OnDrawGizmos()
        {
            if (lineStart == null ||
                lineEnd == null ||
                testPoint == null)
            {
                return;
            }

            Vector2 a = lineStart.position;
            Vector2 b = lineEnd.position;
            Vector2 p = testPoint.position;

            float cross = CalculateCross(a, b, p);

            if (cross > epsilon)
            {
                Gizmos.color = Color.green; // 왼쪽
            }
            else if (cross < -epsilon)
            {
                Gizmos.color = Color.red;   // 오른쪽
            }
            else
            {
                Gizmos.color = Color.yellow; // 직선 위
            }

            Gizmos.DrawLine(lineStart.position, lineEnd.position);
            Gizmos.DrawSphere(testPoint.position, 0.15f);
        }

        private static float CalculateCross(
            Vector2 a,
            Vector2 b,
            Vector2 p)
        {
            // TODO 1: A → B 벡터를 구하세요.
            Vector2 edge = b - a;

            // TODO 2: A → P 벡터를 구하세요.
            Vector2 toPoint = b - p;


            float cross = (edge.x * toPoint.y) - (edge.y * toPoint.x);
            
            return cross;
        }
    }
}