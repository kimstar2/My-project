using System;
using _01.Scripts.SkillSystem;
using _TevLib.CustomUtility;
using Unity.Behavior;
using UnityEngine;

namespace _01.Scripts.Agent.Enemies.BT.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "Can Use Skill", story: "[Enemy] can use [Skill] to [TargetGO]", category: "Conditions", id: "1767897d811ea065d6fb77cd2e8ef401")]
    public partial class CanUseSkillCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<SkillDataSO> Skill;
        [SerializeReference] public BlackboardVariable<GameObject> TargetGO;

        public override bool IsTrue()
        {
            if (Enemy.Value == null || Skill.Value == null || Enemy.Value.SkillModule == null) return false;
            
            return Enemy.Value.SkillModule.CanUseSkill(Skill.Value.skillIdHash,TargetGO.Value);
        }
    }
}
