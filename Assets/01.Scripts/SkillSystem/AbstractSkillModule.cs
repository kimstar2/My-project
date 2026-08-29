using System;
using System.Collections.Generic;
using System.Linq;
using _TevLib.ModuleSystem;
using UnityEngine;

namespace _01.Scripts.SkillSystem
{
    public abstract class AbstractSkillModule : MonoModule , ISkillModule
    {
        public event Action<int> OnSkillEnd;
        
        protected Dictionary<int , ISkill> _skillDict;
        
        public ISkill CurrentSkill {get; private set;}

        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            _skillDict = GetComponentsInChildren<ISkill>()
                .ToDictionary(s => s.SkillData.skillIdHash);

            foreach (ISkill skill in _skillDict.Values)
                skill.InitializeSkill(this);
        }

        public bool CanUseSkill(int skillId, GameObject target = null)
        {
            if (_skillDict.TryGetValue(skillId, out ISkill skill))
                return skill.CanUseSkill(target);
            
            return false;
        }

        public void UseSkill(int skillId, GameObject target = null)
        {
            if (_skillDict.TryGetValue(skillId, out ISkill skill)) // 받은 id로 스킬을 가져와봄
            {
                if (CurrentSkill is { IsUsing: true }) // 사용중이라면?
                {
                    // 그전 스킬 정리
                    ISkill oldSkill = CurrentSkill; // 현재 스킬을 예전스킬로
                    CurrentSkill = null; // 현재 스킬은 비워놓고
                    oldSkill.OnSkillEnd -= HandleSkillEnd; // 예전 스킬 구독해제
                    oldSkill.StopSkill(); // 스킬 끝
                }
                
                CurrentSkill = skill; // 사용중이 아니라면 현재 스킬은 받은 스킬
                CurrentSkill.OnSkillEnd += HandleSkillEnd; // 스킬 끝에 구독해놓고
                CurrentSkill.UseSkill(target); // 타겟에게 스킬 사용 
            }
        }

        
        // 좀따 normalized 쿨 만들거
        
        private void HandleSkillEnd(ISkill endSkill)
        {
            endSkill.OnSkillEnd -= HandleSkillEnd; // 종료되면 구독해제
            int skillId = endSkill.SkillData.skillIdHash;
            if (endSkill == CurrentSkill)
                CurrentSkill = null;
            OnSkillEnd?.Invoke(skillId);
        }

        public abstract float GetBaseDamage(SkillDataSO skillData); // 플레이어랑 적의 데미지 산출식은 다름
    }
}