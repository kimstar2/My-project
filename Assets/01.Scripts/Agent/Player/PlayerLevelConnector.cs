using System;
using _TevLib.ModuleSystem;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.TimeService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _01.Scripts.Agent.Player
{
    public class PlayerLevelConnector : MonoModule, IAfterInitModule
    {
        [SerializeField] private float[] testBaseExp;
        private HealthModule _healthModule;
        private PlayerInventory _playerInventory;
        private int _level = 0;
        private float minReqExp;
        
        [SerializeField, Range(0.5f, 1f)]
        private float minRequiredExpRatio = 0.75f;
        private float _requiredExpThisLevel;
        
        public UnityEvent<float,float> onExpChanged;
        public UnityEvent<float,float> onMinReqExp;
        public UnityEvent<int> onLevelUp;
        private float Exp => _playerInventory.Exp.Value;
        
        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            _healthModule = owner.GetModule<HealthModule>();
            _playerInventory = owner.GetModule<PlayerInventory>();
        }

        private void Update()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
                LevelUp();
        }

        public void AfterInit()
        {
            _playerInventory.expValueChanged.AddListener(HandleLevelCalc);
            HandleLevelCalc(Exp);
        }

        private void OnDestroy()
        {
            _playerInventory.expValueChanged.RemoveListener(HandleLevelCalc);
        }
        private void HandleLevelCalc(float exp)
        {
            _requiredExpThisLevel = CalculateRequiredExp();
            onMinReqExp?.Invoke(_requiredExpThisLevel, testBaseExp[_level]);
            EvaluateLevel(exp);
        }
        
        private void EvaluateLevel(float exp)
        {
            float baseExp = testBaseExp[_level];
            float visibleExp = Mathf.Min(exp, _requiredExpThisLevel);

            // 검은 면: 현재 EXP / BaseExp
            onExpChanged?.Invoke(visibleExp, baseExp);

            if (exp < _requiredExpThisLevel)
                return;

            LevelUp();
            _playerInventory.ResetExp();
        }

        private void LevelUp()
        {
            _level++;
            onLevelUp?.Invoke(_level);
            ServiceLocator.GetService<ITimeService>().SetTimeScale(0f);
        }

        private float CalculateRequiredExp()
        {
            float healthPer = _healthModule.GetHealthPer();
            float multiplier = minRequiredExpRatio + (1f-minRequiredExpRatio) * healthPer;
            return testBaseExp[_level] * multiplier;
        }

        public void ReCalc()
        {
            float exp = Exp;
            float recalculatedExp = CalculateRequiredExp();

            _requiredExpThisLevel = Mathf.Min(
                _requiredExpThisLevel,
                recalculatedExp);

            onMinReqExp?.Invoke(
                _requiredExpThisLevel,
                testBaseExp[_level]);

            EvaluateLevel(exp);
        }
    }
}
