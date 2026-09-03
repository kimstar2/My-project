using _01.Scripts.Agent.Interface;
using _01.Scripts.CombatSystem;
using _01.Scripts.ItemSystem;
using _01.Scripts.SkillSystem;
using _TevLib.CoreLib;
using _TevLib.CoreLib.EventSystem;
using _TevLib.ModuleSystem;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.Agent.Player.Skill
{
    public class PlayerDefaultAttack : AbstractSkill
    {
        [field:SerializeField] public StatItemType ApplyStat { get; private set; }
        [SerializeField] private EventChannelSO eventChannelSO;
        private IRenderable _renderer;
        private IAnimatorTrigger _trigger;
        private AbstractDamageCaster _damageCaster;
        public NotifyValue<float> Damage { get; private set; } = new();
        public UnityEvent onAttack;
        public UnityEvent onHit;
        public UnityEvent<float> onBindDamage;


        public override void InitializeSkill(ISkillModule skillModule)
        {
            base.InitializeSkill(skillModule);
            RuntimeSkillData = Instantiate(SkillData);
            Damage.Value = skillModule.GetBaseDamage(RuntimeSkillData);
            
            GetReqModule(skillModule.Owner);

            _damageCaster = GetComponentInChildren<AbstractDamageCaster>();
            _damageCaster?.InitCaster(skillModule.Owner);
            onBindDamage?.Invoke(SkillModule.GetBaseDamage(SkillData));
        }
        
        public void SetDamage(float value)
        {
            Damage.Value = value;
            onBindDamage?.Invoke(Damage.Value);
        }

        #region Skill

        public override bool CanUseSkill(GameObject target = null) => !IsUsing;

        public override void UseSkill(GameObject target = null)
        {
            base.UseSkill(target);

            onAttack?.Invoke();

            _trigger.OnDamageCast -= HandleDamageCast;
            _trigger.OnDamageCast += HandleDamageCast;
            _trigger.OnAnimationEnd -= HandleAnimEnd;
            _trigger.OnAnimationEnd += HandleAnimEnd;

            if (RuntimeSkillData.defaultAnimHash != null)
                _renderer.RenderClip(SkillData.defaultAnimHash.HashValue);
        }

        private void HandleAnimEnd()
            => CleanUpSkillData();

        private void HandleDamageCast()
        {
            Vector2 direction = _renderer.FacingDirection;
            _damageCaster.transform.position = transform.position + (Vector3)direction * SkillData.maxRange * 0.5f;

            bool hit = _damageCaster.CastDamage(Damage.Value, direction, SkillData.kbForce);
            if (hit)
                onHit?.Invoke();
        }

        public override void CleanUpSkillData()
        {
            _trigger.OnDamageCast -= HandleDamageCast;
            _trigger.OnAnimationEnd -= HandleAnimEnd;
            base.CleanUpSkillData();
        }

        #endregion

        #region InitTime

        private void GetReqModule(ModuleOwner owner)
        {
            _renderer = owner.GetModule<IRenderable>();
            _trigger =owner.GetModule<IAnimatorTrigger>();
            Debug.Assert(_renderer != null, "_renderer is null");
            Debug.Assert(_trigger != null, "_trigger is null");
            Debug.Assert(_renderer != null, "_playerStatApplier is null");
        }

        #endregion
    }
}