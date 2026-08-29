using System;
using _01.Scripts.Agent.Interface;
using _TevLib.HashDataSystem;
using _TevLib.ModuleSystem;
using UnityEngine;

namespace _01.Scripts.Agent
{
    [RequireComponent(typeof(Animator))]
    public class AgentRenderer : MonoModule, IRenderable, IAnimatorTrigger
    {
        [SerializeField] private AnimHashSO moveXHash;
        [SerializeField] private AnimHashSO moveYHash;
        public Animator AnimCompo { get; private set; }
        public Vector2 FacingDirection { get; private set; }


        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            AnimCompo = GetComponent<Animator>();
        }
        
        public void SetMovementDirection(Vector2 direction)
        {
            if (Mathf.Approximately(direction.magnitude, 0))
                return;

            AnimCompo?.SetFloat(moveXHash.HashValue, direction.x);
            AnimCompo?.SetFloat(moveYHash.HashValue, direction.y);
            FacingDirection = direction;
        }

        public void RenderClip(int clipHash)
        {
            AnimCompo?.Play(clipHash,0,0);
        }

        public void RenderClipIfNotPlaying(int clipHash)
        {
            if (AnimCompo.GetCurrentAnimatorStateInfo(0).shortNameHash != clipHash)
                RenderClip(clipHash);
        }

        #region AnimTrigger

        public event Action OnAnimationEnd;
        public event Action OnDamageCast;
        public event Action OnFootstep;

        private void HandleAnimationEnd() => OnAnimationEnd?.Invoke();
        private void HandleDamageCast() => OnDamageCast?.Invoke();
        private void HandleFootstep() => OnFootstep?.Invoke();
        #endregion
    }
}
