using System;
using System.Threading;
using _01.Scripts.Agent.Interface;
using _TevLib.ModuleSystem;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.TimeService;
using UnityEngine;

namespace _01.Scripts.Agent
{
    public class AgentMovement : MonoModule , IMoveable
    {
        [field:SerializeField] private bool useAcceleration;
        [field:SerializeField] public float MoveSpeed { get; private set; } = 5f;
        [field:SerializeField] private bool useMaxMoveSpeed;
        [field:SerializeField] private float maxMoveSpeed = 15f;
        [field:SerializeField] private float acceleration;
        private ITimeService _timeService;
        private bool _overrideVelocity = true;
        private CancellationTokenSource _addForceCts;
        
        public Rigidbody2D RbCompo {get; private set;}
        public Vector2 MoveDirection {get; private set;}
        
        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            RbCompo = Owner.GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _timeService = ServiceLocator.GetService<ITimeService>();
        }

        private void FixedUpdate()
        {
            if (!_overrideVelocity) return;
            
            Vector2 targetVelocity = MoveDirection * MoveSpeed;
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
        
        public void SetSpeed(float speed) => MoveSpeed = Mathf.Clamp(speed, 0, useMaxMoveSpeed ? maxMoveSpeed : float.PositiveInfinity);

        public void SetDirection(Vector2 direction)
        {
            MoveDirection = direction.normalized;
        }

        public void AddForce(Vector2 force, float ignoreVelocityTime, ForceMode2D forceMode = ForceMode2D.Force)
        {
            KillTask();
            _addForceCts =  new CancellationTokenSource();
            CancellationToken ct = _addForceCts.Token;
            
            _timeService.ActionTimer(ignoreVelocityTime, ct,
                () => _overrideVelocity = false,
                () => _overrideVelocity = true);
            StopImmediately();
            RbCompo.AddForce(force, forceMode);
        }

        public void StopImmediately()
        {
            SetDirection(Vector2.zero);
            RbCompo.linearVelocity = Vector2.zero;
        }

        private void OnDisable()
        {
            KillTask();
            _overrideVelocity = true;
        }

        private void KillTask()
        {
            if (_addForceCts != null)
            {
                _addForceCts.Cancel();
                _addForceCts.Dispose();
                _addForceCts = null;
            }
        }
    }
}