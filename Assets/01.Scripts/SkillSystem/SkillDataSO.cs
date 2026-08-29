using _TevLib.HashDataSystem;
using UnityEngine;

namespace _01.Scripts.SkillSystem
{    
    public enum SkillCategory
    {
        BasicAttack,
        Active
    }
    public enum SkillType
    {
        Physical, Magic, NonDamage
    }
    public enum DirectionType
    {
        Body, Pointer
    }
    [CreateAssetMenu(fileName = "Skill data", menuName = "Agent/Skill data", order = 0)]
    public class SkillDataSO : ScriptableObject
    {
        public string skillName;
        public int skillIdHash;
        public Sprite icon;
        
        [Header("Skill Settings")]
        public SkillCategory skillCategory = SkillCategory.Active;
        public SkillType skillType = SkillType.Physical;
        public DirectionType directionType =  DirectionType.Body;
        public bool canMove;
        public float maxRange;
        public AnimHashSO defaultAnimHash;
        public float damageMultiplier = 1f;
        public float baseSkillDamage = 3f;
        public float kbForce = 0f;
        public float cooldownTime = 0.5f;

        private void OnValidate()
        {
            skillIdHash = Animator.StringToHash(skillName);
        }
    }
}