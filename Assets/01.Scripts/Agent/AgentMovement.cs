using System;
using System.Threading;
using _01.Scripts.Agent.Interface;
using _01.Scripts.Util;
using _TevLib.ModuleSystem;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.TimeService;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace _01.Scripts.Agent
{
    public class AgentMovement : MonoModule , IMoveable
    {
        [field:Header("Move Settings")]
        [field:HorizontalLineAttribute(color: EColor.Gray)]
        [field:SerializeField] public float MoveSpeed { get; private set; } = 5f;
        [SerializeField] private bool useMaxMoveSpeed;
        [SerializeField] private float maxMoveSpeed = 15f;
        
        [Header("Acceleration Settings")]
        [field:HorizontalLine(color: EColor.Gray)]
        [SerializeField] private bool useAcceleration;
        [SerializeField] private float acceleration;
        private Tween _addForceTween;
        private Vector2 _dirAddForce;
        
        public Rigidbody2D RbCompo {get; private set;}
        public Vector2 MoveDirection {get; private set;}
        
        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            RbCompo = Owner.GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            Vector2 targetVelocity = MoveDirection * MoveSpeed + _dirAddForce;
            if (useAcceleration)
            {
                RbCompo.linearVelocity = Vector2.MoveTowards(
                    RbCompo.linearVelocity,
                    targetVelocity,
                    acceleration * Time.fixedDeltaTime);
                // fixedDeltaTime => 다음프레임 예측
            }
            else
                RbCompo.linearVelocity = targetVelocity; 
        }
        
        private void OnDestroy()
        {
            _addForceTween?.Kill();
        }

        #region Velocity
        
        public void SetSpeed(float speed) => MoveSpeed = Mathf.Clamp(speed, 0, useMaxMoveSpeed ? maxMoveSpeed : float.PositiveInfinity);

        public void SetDirection(Vector2 direction)
        {
            MoveDirection = direction.normalized;
        }

        public void StopImmediately()
        {
            SetDirection(Vector2.zero);
            RbCompo.linearVelocity = Vector2.zero;
        }

        public void Stop() 
            => SetDirection(Vector2.zero);
        
        #endregion

        #region Addforce
        
        public void AddForce(Vector2 force, float ignoreVelocityTime, Ease addForceEase , ForceMode2D forceMode = ForceMode2D.Force)
        {
            _addForceTween?.Kill();
            _dirAddForce = force;
            
            _addForceTween = DOTween.To(() => _dirAddForce,
                x => _dirAddForce = x,
                Vector2.zero, ignoreVelocityTime).
                SetEase(addForceEase).
                SetAutoKill(true);
        }

        public void AddForce(Vector2 force, float ignoreVelocityTime, AnimationCurve addForceCurve , ForceMode2D forceMode = ForceMode2D.Force)
        {
            _addForceTween?.Kill();
            _dirAddForce = force;
            
            _addForceTween = DOTween.To(() => _dirAddForce,
                    x => _dirAddForce = x,
                    Vector2.zero, ignoreVelocityTime).
                SetEase(addForceCurve).
                SetAutoKill(true);
        }
        
        #endregion
    }
}