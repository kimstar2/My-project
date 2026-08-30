using System;
using _01.Scripts.Agent.Interface;
using _01.Scripts.CombatSystem;
using _01.Scripts.GameSystem.GameServices;
using _01.Scripts.SkillSystem;
using _TevLib.CoreLib.EventSystem;
using _TevLib.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.Agent.Player.Skill
{
    public class PlayerDefaultAttack : AbstractSkill
    {
        public UnityEvent onAttack;
        public UnityEvent onHit;
        private IRenderable _renderer;
        private IAnimatorTrigger _trigger;
        private AbstractDamageCaster _damageCaster;
        public UnityEvent<float> onBindDamage;
        
        public override void InitializeSkill(ISkillModule skillModule)
        {
            base.InitializeSkill(skillModule);
            _renderer = skillModule.Owner.GetModule<IRenderable>();
            _trigger = skillModule.Owner.GetModule<IAnimatorTrigger>();
            Debug.Assert(_renderer != null, "_renderer is null");
            Debug.Assert(_trigger != null, "_trigger is null");
            
            _damageCaster = GetComponentInChildren<AbstractDamageCaster>();
            _damageCaster?.InitCaster(skillModule.Owner);
            onBindDamage?.Invoke(SkillModule.GetBaseDamage(SkillData));
        }

        private void OnEnable()
        {
            SkillData.NotifyBaseSkillDamage.OnValueChanged += HandleNotifyDamageChanged;
            SkillData.NotifyDamageMultiplier.OnValueChanged += HandleNotifyDamageChanged;
        }
        
        private void OnDisable()
        {
            SkillData.NotifyBaseSkillDamage.OnValueChanged -= HandleNotifyDamageChanged;
            SkillData.NotifyDamageMultiplier.OnValueChanged -= HandleNotifyDamageChanged;
        }
        private void HandleNotifyDamageChanged(float prev, float next) => onBindDamage?.Invoke(SkillModule.GetBaseDamage(SkillData));

        public override bool CanUseSkill(GameObject target = null) => !IsUsing;

        public override void UseSkill(GameObject target = null)
        {
            base.UseSkill(target);
            
            onAttack?.Invoke();
            
            _trigger.OnDamageCast -= HandleDamageCast;
            _trigger.OnDamageCast += HandleDamageCast;
            _trigger.OnAnimationEnd -= HandleAnimEnd;
            _trigger.OnAnimationEnd += HandleAnimEnd;
            
            if (SkillData.defaultAnimHash != null)
                _renderer.RenderClip(SkillData.defaultAnimHash.HashValue);
        }

        private void HandleAnimEnd() 
            => CleanUpSkillData();

        private void HandleDamageCast()
        {
            Vector2 direction = _renderer.FacingDirection;
            _damageCaster.transform.position = transform.position + (Vector3)direction * SkillData.maxRange * 0.5f;
            float damage = SkillModule.GetBaseDamage(SkillData);
            
            bool hit = _damageCaster.CastDamage(damage,direction , SkillData.kbForce);
            if (hit)
                onHit?.Invoke();
        }

        public override void CleanUpSkillData()
        {
            _trigger.OnDamageCast -= HandleDamageCast;
            _trigger.OnAnimationEnd -= HandleAnimEnd;
            base.CleanUpSkillData();
        }
    }
}