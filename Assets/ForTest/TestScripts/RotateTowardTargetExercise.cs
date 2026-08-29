using UnityEngine;

namespace _02.Scenes.ForTest.TestScripts
{
    public class RotateTowardTargetExercise : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float epsilon = 0.001f;

        private void Update()
        {
            if (target == null)
                return;

            Vector2 forward = transform.right;
            Vector2 toTarget = target.position - transform.position;

            float turnDirection = GetTurnDirection(forward, toTarget);

            transform.Rotate(
                0f,
                0f,
                turnDirection * rotationSpeed * Time.deltaTime);
        }

        private float GetTurnDirection(
            Vector2 forward,
            Vector2 toTarget)
        {
            float cross = forward.x * toTarget.y - forward.y * toTarget.x;

            if (cross > epsilon)
                return 1f;
            if (cross < -epsilon)
                return -1f;
            
            
            float dot = forward.x * toTarget.x + forward.y * toTarget.y;
            if (dot < 0f)
                return 1f;
            
            return 0f;
        }

        private void OnDrawGizmos()
        {
            if (target == null)
                return;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.right * 2f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}