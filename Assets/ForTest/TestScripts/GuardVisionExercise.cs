using TMPro;
using UnityEngine;

namespace ForTest.TestScripts
{
    public class GuardVisionExercise : MonoBehaviour
    {
        private enum TargetState
        {
            OutOfRange,
            OutsideView,
            Center,
            Left,
            Right
        }

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Transform target;
        [SerializeField] private Transform facingTarget;
        [SerializeField] private float viewDistance = 5f;
        [SerializeField] private float minimumViewAngle = 45f;

        private void Update()
        {
            TargetState state = EvaluateTarget();
            text.SetText($"Now : {state}");
        }

        private TargetState EvaluateTarget()
        {
            Vector2 targetDir = target.position - transform.position;
            Vector2 direction = targetDir.normalized;
            Vector2 facing = transform.right;
            
            float sqrDistance = targetDir.sqrMagnitude;
            float sqrViewDistance = viewDistance * viewDistance;
            
            if (sqrDistance > sqrViewDistance)
                return TargetState.OutOfRange;
            
            if (sqrDistance <= float.Epsilon)
                return TargetState.Center;
            
            float dotProduct = facing.x * direction.x + facing.y * direction.y;

            if (dotProduct < minimumViewAngle * Mathf.Deg2Rad)
                return TargetState.OutsideView;
            
            float cross = facing.x * direction.y - facing.y * direction.x;

            if (cross > float.Epsilon)
                return TargetState.Left;
            if (cross < -float.Epsilon)
                return TargetState.Right;
            
            return TargetState.Center;
        }

        private void OnDrawGizmos()
        {
            TargetState nowVisionState = EvaluateTarget();
            Gizmos.color = nowVisionState switch
            {
                TargetState.OutOfRange => Color.gray,
                TargetState.OutsideView => Color.red,
                TargetState.Center => Color.yellow,
                TargetState.Left => Color.green,
                TargetState.Right => Color.blue,
                _ => Color.black
            };
            
            Gizmos.DrawLine(transform.position, target.position);
            
            Vector3 position = transform.position;
            Vector3 facing = transform.right;

            // 코사인 값 → 라디안 각도 → 도 단위 각도
            float halfAngle = Mathf.Acos(minimumViewAngle * Mathf.Deg2Rad) * Mathf.Rad2Deg;

            Vector3 leftDirection =
                Quaternion.Euler(0f, 0f, halfAngle) * facing;

            Vector3 rightDirection =
                Quaternion.Euler(0f, 0f, -halfAngle) * facing;

            Gizmos.color = Color.crimson;
            Gizmos.DrawRay(
                position,
                facing * viewDistance);

            Gizmos.color = Color.white;
            Gizmos.DrawRay(
                position,
                leftDirection * viewDistance);

            Gizmos.DrawRay(
                position,
                rightDirection * viewDistance);
            
            Gizmos.color = Color.aquamarine;
            Gizmos.DrawWireSphere(transform.position, viewDistance);

            Gizmos.color = Color.blueViolet;
            Gizmos.DrawLine(transform.position, facingTarget.position);
            Gizmos.DrawWireSphere(facingTarget.position, 0.25f);
        }
    }
}