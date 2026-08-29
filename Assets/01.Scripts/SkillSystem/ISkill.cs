using System;
using UnityEngine;

namespace _01.Scripts.SkillSystem
{
    public interface ISkill
    {
        event Action<ISkill> OnSkillEnd;
        SkillDataSO SkillData { get; }
        float NormalizedCooldown { get; } // 0~1 로 표기
        bool IsUsing { get; }
        bool CanInterrupt { get; } // 캔슬 가능한 스킬이냐?
        
        void InitializeSkill(ISkillModule skillModule);
        bool CanUseSkill(GameObject target = null);
        void UseSkill(GameObject target = null);
        
        void OnUpdateSkill(); // 차징 이나 누적타이머를 위한
        void OnReleaseInput();
        void StopSkill();
        void CleanUpSkillData();
    }
}