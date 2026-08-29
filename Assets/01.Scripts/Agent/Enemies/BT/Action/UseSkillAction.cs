using System;
using _01.Scripts.SkillSystem;
using _TevLib.CustomUtility;
using _TevLib.HashDataSystem;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace _01.Scripts.Agent.Enemies.BT.Action
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Use Skill", story: "[Enemy] Use [Skill] to [TargetGO]", category: "Action/Combat",
        id: "c8fc700c5d9127a7ef5797716df55127")]
    public partial class UseSkillAction : Unity.Behavior.Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<SkillDataSO> Skill;
        [SerializeReference] public BlackboardVariable<GameObject> TargetGO;

        private ISkillModule _skillModule;
        private bool _skillComplete;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Skill.Value == null || Enemy.Value.SkillModule == null)
                return Status.Failure;

            _skillModule = Enemy.Value.SkillModule;
            if (_skillModule == null)
                return Status.Failure;

            int skillId = Skill.Value.skillIdHash;
            if (!_skillModule.CanUseSkill(skillId, TargetGO.Value))
                return Status.Failure;
            
            _skillComplete = false;
            _skillModule.OnSkillEnd -= HandleSkillEnd;
            _skillModule.OnSkillEnd += HandleSkillEnd;
            _skillModule.UseSkill(skillId , TargetGO.Value);
            Debug.Log("Skill Use");
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _skillComplete ? Status.Success : Status.Running;
        }
        
        protected override void OnEnd()
        {
            if (_skillModule != null)
            {
                _skillModule.OnSkillEnd -= HandleSkillEnd;
                _skillModule.CurrentSkill?.CleanUpSkillData();
            }
            
        }
        private void HandleSkillEnd(int skillId)
        {
            if (skillId == Skill.Value.skillIdHash)
                _skillComplete = true;
        }
    }
}

