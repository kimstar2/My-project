using System;
using _01.Scripts.Agent.Interface;
using _TevLib.ModuleSystem;
using UnityEngine;

namespace _01.Scripts.Agent
{
    public class AgentMovement : MonoModule , IMoveable
    {
        public event Action<Vector2> OnMoveDirChange;
        [SerializeField] private bool useAcceleration;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float maxMoveSpeed = 15f;
        [SerializeField] private float acceleration;
        
        public Rigidbody2D RbCompo {get; private set;}
        public Vector2 MoveDirection {get; private set;}
        
        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            RbCompo = Owner.GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            Vector2 targetVelocity = MoveDirection * moveSpeed;
            if (useAcceleration)
            {
                RbCompo.linearVelocity = Vector2.MoveTowards(
                    RbCompo.linearVelocity,
                    targetVelocity,
                    acceleration * Time.fixedDeltaTime);
            }
            else
                RbCompo.linearVelocity = targetVelocity; 
        }

        
        public void SetMovementSpeed(float speed) => moveSpeed = Mathf.Clamp(speed, 0, maxMoveSpeed);

        public void SetDirection(Vector2 direction)
        {
            MoveDirection = direction.normalized;
            OnMoveDirChange?.Invoke(MoveDirection);
        }

        public void StopImmediately()
        {
            SetDirection(Vector2.zero);
            RbCompo.linearVelocity = Vector2.zero;
        }
    }
}