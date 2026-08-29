using System;
using UnityEngine;

namespace _02.Scenes.ForTest.TestScripts
{
    public class TestRotateToTarget : MonoBehaviour
    {
        [SerializeField] private Transform facingTarget;
        [SerializeField] private bool isRotate;
        
        private void Update()
        {
            if (!isRotate || facingTarget == null) return;
            FacingTarget();
        }

        private void FacingTarget()
        {
            Vector2 targetDir = facingTarget.position - transform.position;
            Vector2 direction = targetDir.normalized;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }
}