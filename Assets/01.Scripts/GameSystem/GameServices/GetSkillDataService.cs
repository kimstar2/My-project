using System;
using _01.Scripts.SkillSystem;
using _TevLib.ServiceLocatorSystem;
using UnityEngine;

namespace _01.Scripts.GameSystem.GameServices
{
    public class GetSkillDataService : MonoBehaviour , IGetSkillData
    {
        public SkillDataSO SkillData {get; private set;}

        private void Awake() => ServiceLocator.RegisterService<IGetSkillData>(this);
        private void OnDestroy() => ServiceLocator.UnregisterService<IGetSkillData>();
        
        public void SetSkillData(SkillDataSO skillData) => SkillData = skillData;
    }
}