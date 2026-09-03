using System;
using UnityEngine;

namespace _01.Scripts.SkillSystem
{
    public abstract class AbstractSkill : MonoBehaviour , ISkill
    {
        public event Action<ISkill> OnSkillEnd;
        [field:SerializeField] public SkillDataSO SkillData { get; private set; }
        public SkillDataSO RuntimeSkillData {get; protected set;}

        protected float LastUsedTime = float.NegativeInfinity;
        protected ISkillModule SkillModule;
        
        public float NormalizedCooldown
        {
            get
            {
                if (SkillData == null || SkillData.cooldownTime <= 0) return 0f;
                return Mathf.Clamp01(1f - (Time.time - LastUsedTime) / SkillData.cooldownTime);
            }
        }

        public bool IsUsing { get; private set; }
        public virtual bool CanInterrupt => false;
        
        public virtual void InitializeSkill(ISkillModule skillModule)
        => SkillModule = skillModule;

        public abstract bool CanUseSkill(GameObject target = null);

        public virtual void UseSkill(GameObject target = null) => IsUsing = true;
        public virtual void OnUpdateSkill() { }

        public virtual void OnReleaseInput() { }

        public void StopSkill() => CleanUpSkillData();
        
        public virtual void CleanUpSkillData()
        {
            LastUsedTime = Time.time;
            IsUsing = false;
            OnSkillEnd?.Invoke(this);
        }
    }
}