using System;
using System.Collections.Generic;
using System.Linq;
using _01.Scripts.GameSystem;
using _01.Scripts.ItemSystem;
using _01.Scripts.SkillSystem;
using _TevLib.ModuleSystem;
using UnityEngine;

namespace _01.Scripts.Agent.Player
{
    public class PlayerSkillModule : AbstractSkillModule , IAfterInitModule
    {
        [Serializable]
        public struct LoadoutEntry // 기본스킬
        {
            public SkillSlot slot; // 스킬 슬롯 (Q,E..)
            public SkillDataSO skillData; // 스킬 데이터
        }
        
        [Tooltip("시작 시 각 슬롯에 장착될 기본 스킬 (런타임에 장착도 가능하게 할 거)")]
        [SerializeField] private List<LoadoutEntry> _defaultLoadout = new List<LoadoutEntry>();
        
        // 어떤 슬롯에 어떤 스킬이 장착 되었는가
        private readonly Dictionary<SkillSlot,int> _slotToSkillId = new Dictionary<SkillSlot, int>();
        
        // 기본 공격 관련
        private int _basicAttackId;
        private bool _hasBasicAttack;

                
        // 가장 최근에 요청된 스킬 ID, FSM 에서 Skill 진입시 이 스킬을 시전
        public int RequestedSkillId {get; private set;}
        public SkillSlot? RequestedInputSlot {get; private set;}
        
        public event Action<SkillSlot, SkillDataSO> OnSlotChanged; // UI 가 구독해서 처리한다.
        
        // 스킬 딕셔너리가 다 채워지고 (base.Init에서) 그 뒤에 호출해서 정리 작업을 수행
        public void AfterInit()
        {
            CacheBasicAttack();
            BuildDefaultLoadOut();
        }

        private void CacheBasicAttack()
        {
            ISkill basicSkill = _skillDict.Values
                .FirstOrDefault(s => s.SkillData.skillCategory == SkillCategory.BasicAttack);

            if (basicSkill == null)
            {
                Debug.LogWarning($"[PlayerSkillModule] 기본 공격이 누락 되었습니다. : {gameObject}");
                return;
            }

            _basicAttackId = basicSkill.SkillData.skillIdHash;
            _hasBasicAttack = true; // 기본 공격 소유중
        }

        private void BuildDefaultLoadOut()
        {
            foreach (LoadoutEntry entry in _defaultLoadout)
            {
                if (entry.skillData == null) continue;
                EquipSkill(entry.slot, entry.skillData.skillIdHash);
            }
        }

        #region 키 입력 해석 및 시전 요정 (FSM 에서 요청)

        public bool TryResolveBasicAttack(out int skillId)
        {
            skillId = _basicAttackId;
            return _hasBasicAttack;
        }

        public bool TryResolveSlot(SkillSlot slot, out int skillId)
        {
            return _slotToSkillId.TryGetValue(slot , out skillId);
        }
        
        // 시전 가능여부 검사후 가능하다면 요청 기록해주는 함수
        public bool TryRequestSkill(int skillId , SkillSlot? inputSlot)
        {
            if (CurrentSkill is {IsUsing: true, CanInterrupt: false}) return false;
            
            if (!CanUseSkill(skillId)) return false;
            RequestedSkillId = skillId;
            RequestedInputSlot = inputSlot;
            return true;    
        }
        
        #endregion
        
        #region UI 연동중 API

        public void EquipSkill(SkillSlot entrySlot, int skillId)
        {
            if (!_skillDict.TryGetValue(skillId, out ISkill skill))
            {
                Debug.LogWarning($"[PlayerSkillModule] 장착하려는 스킬이 없습니다 : {skillId}");
                return;
            }
            _slotToSkillId[entrySlot] = skillId;
            OnSlotChanged?.Invoke(entrySlot, skill.SkillData);

        }
        public void UnequipSkill(SkillSlot entrySlot)
        {
            if (_slotToSkillId.Remove(entrySlot))
                OnSlotChanged?.Invoke(entrySlot, null);
        }
        
        // 스킬에 장착된 스킬 데이터를 가져오는
        public SkillDataSO GetSkillData(SkillSlot slot)
        {
            return _slotToSkillId.TryGetValue(slot, out int skillId)
                   && _skillDict.TryGetValue(skillId, out ISkill skill)
                   ? skill.SkillData
                   : null;
        }

        public SkillDataSO GetSkillData(int skillId) => _skillDict.TryGetValue(skillId, out ISkill skill)? skill.SkillData : null;

        // UI 관련 쿨다임 표기 로직만 없는 상태
        public float GetSlotCooldown(SkillSlot slot)
        {
            return 0.5f;
        }
        
        #endregion
        
        public override float GetBaseDamage(SkillDataSO skillData)
        {
            float defaultDamage = skillData.NotifyBaseSkillDamage.Value * skillData.NotifyDamageMultiplier.Value;
            return defaultDamage;
        }
    }
}