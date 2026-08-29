using _01.Scripts.SkillSystem;
using UnityEngine;

namespace _01.Scripts.Agent.Enemies.Skill
{
    public class EnemySkillModule : AbstractSkillModule
    {
        public override float GetBaseDamage(SkillDataSO skillData)
        {
            return skillData.baseSkillDamage * skillData.damageMultiplier;
        }
    }
}