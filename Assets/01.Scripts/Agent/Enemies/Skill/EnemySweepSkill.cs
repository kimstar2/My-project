using _01.Scripts.Agent.Interface;
using _01.Scripts.CombatSystem;
using _01.Scripts.SkillSystem;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.Agent.Enemies.Skill
{
    public class EnemySweepSkill : AbstractSkill
    {
        private IRenderable _renderer;
        private IAnimatorTrigger _trigger;
        private IMoveable _mover;
        private AbstractDamageCaster _damageCaster;
        
        public UnityEvent onAttack;
        public UnityEvent onHit;
        public UnityEvent onFail;

        public override void InitializeSkill(ISkillModule skillModule)
        {
            base.InitializeSkill(skillModule);
            _renderer = skillModule.Owner.GetModule<IRenderable>();
            _trigger = skillModule.Owner.GetModule<IAnimatorTrigger>();
            _mover = skillModule.Owner.GetModule<IMoveable>();
            _damageCaster = GetComponentInChildren<AbstractDamageCaster>();
            _damageCaster?.InitCaster(skillModule.Owner);
        }

        public override bool CanUseSkill(GameObject target = null)
        {
            if (target == null) return false;
            if (IsUsing || NormalizedCooldown > 0f ) return false; // 사용중이거나 쿨타임이면
            
            return Vector2.Distance(transform.position, target.transform.position) <= SkillData.maxRange;
        }

        public override void UseSkill(GameObject target = null)
        {
            base.UseSkill(target);
            _mover.StopImmediately();

            if (target != null && SkillData.directionType == DirectionType.Body)
            {
                Vector2 direction = target.transform.position - transform.position;
                _renderer.SetMovementDirection(direction.normalized);
            }

            _trigger.OnAnimationEnd -= HandleSkillAnimationEnd;
            _trigger.OnAnimationEnd += HandleSkillAnimationEnd;

            _trigger.OnDamageCast -= HandleDamageCast;
            _trigger.OnDamageCast += HandleDamageCast;
            
            if (SkillData.defaultAnimHash != null)
                _renderer.RenderClip(SkillData.defaultAnimHash.HashValue);
        }

        private void HandleDamageCast()
        {
            Vector2 direction = _renderer.FacingDirection;
            _damageCaster.transform.position = transform.position + (Vector3)direction * SkillData.maxRange * 0.5f;
            float damage = SkillModule.GetBaseDamage(SkillData);
            
            bool hit = _damageCaster.CastDamage(damage,direction,SkillData.kbForce);
            
            onAttack?.Invoke();
            if (hit)
                onHit?.Invoke();
            else
                onFail?.Invoke();
        }

        private void HandleSkillAnimationEnd()
        {
            StopSkill();
        }

        public override void CleanUpSkillData()
        {
            _trigger.OnAnimationEnd -= HandleSkillAnimationEnd;
            _trigger.OnDamageCast -= HandleDamageCast;
            base.CleanUpSkillData();
        }
    }
}