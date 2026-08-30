using _01.Scripts.SkillSystem;

namespace _01.Scripts.GameSystem.GameServices
{
    public interface IGetSkillData
    {
        SkillDataSO SkillData { get; }
        void SetSkillData(SkillDataSO skillData);
    }
}