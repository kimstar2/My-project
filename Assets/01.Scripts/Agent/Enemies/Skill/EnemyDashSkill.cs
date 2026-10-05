using System.Collections.Generic;
using _01.Scripts.Agent.Interface;
using _01.Scripts.CombatSystem;
using _01.Scripts.SkillSystem;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.Agent.Enemies.Skill
{
    public class EnemyDashSkill : AbstractSkill
    {
        [SerializeField] private float dashRange = 5f;
        [SerializeField] private float dashTime = 2f;
        
        private IRenderable _renderer;
        private IAnimatorTrigger _trigger;
        private IMoveable _mover;
        private AbstractTickDamageCaster _damageCaster;
        
        public UnityEvent onStartDamageCast;
        public UnityEvent onEndDamageCast;
        
        public override void InitializeSkill(ISkillModule skillModule)
        {
            base.InitializeSkill(skillModule);
            _renderer = skillModule.Owner.GetModule<IRenderable>();
            _trigger = skillModule.Owner.GetModule<IAnimatorTrigger>();
            _mover = skillModule.Owner.GetModule<IMoveable>();
            _damageCaster = GetComponentInChildren<AbstractTickDamageCaster>();
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
                Vector2 normalizeDir = direction.normalized;
                
                _mover.AddForce(normalizeDir * dashRange, dashTime,ForceMode2D.Impulse);
                _renderer.SetMovementDirection(normalizeDir);
            }

            _trigger.OnAnimationEnd -= HandleSkillAnimationEnd;
            _trigger.OnAnimationEnd += HandleSkillAnimationEnd;

            _trigger.OnStartDamageCast -= HandleStartDamageCast;
            _trigger.OnStartDamageCast += HandleStartDamageCast;
            
            if (SkillData.defaultAnimHash != null)
                _renderer.RenderClip(SkillData.defaultAnimHash.HashValue);
        }

        private void HandleStartDamageCast()
        {
            Vector2 direction = _renderer.FacingDirection;
            float damage = SkillModule.GetBaseDamage(SkillData);
            
            _damageCaster.OnCastDamage(damage,direction,SkillData.kbForce);
            
            onStartDamageCast?.Invoke();
        }
        
        private void HandleEndDamageCast()
        {
            _damageCaster.OffCastDamage();
            onEndDamageCast?.Invoke();
        }

        private void HandleSkillAnimationEnd()
        {
            StopSkill();
        }

        public override void CleanUpSkillData()
        {
            _trigger.OnAnimationEnd -= HandleSkillAnimationEnd;
            _trigger.OnStartDamageCast -= HandleStartDamageCast;
            base.CleanUpSkillData();
        }
    }
}