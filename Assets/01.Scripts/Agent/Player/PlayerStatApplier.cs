using System;
using System.Collections.Generic;
using _01.Scripts.Agent.Interface;
using _01.Scripts.GameSystem.Event;
using _01.Scripts.GameSystem.GameServices;
using _01.Scripts.ItemSystem;
using _TevLib.CoreLib.EventSystem;
using _TevLib.ModuleSystem;
using _TevLib.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.Agent.Player
{
    [Serializable]
    public struct ApplyStatableStruct
    {
        [field:SerializeField] public StatItemType ApplyStat {get; private set;}
        [field:SerializeField] public float BaseValue {get; private set;}
        [field:SerializeField] public UnityEvent<float> ApplyValue {get; private set;}

    }
    public class PlayerStatApplier : MonoModule
    {
        [SerializeField] private EventChannelSO eventChannelSO;
        [SerializeField] public List<ApplyStatableStruct> applyStatableList;
        private PlayerStatGetter _statGetter;

        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            _statGetter = Owner.GetModule<PlayerStatGetter>(); 
        }

        private void OnEnable() => eventChannelSO.AddListener<OnApplyStat>(HandleApplyStat);
        private void OnDisable() => eventChannelSO.RemoveListener<OnApplyStat>(HandleApplyStat);

        private void Start()
        {
            foreach (ApplyStatableStruct applyStatable in applyStatableList)
            {
                float resultValue = _statGetter.GetStat(applyStatable.ApplyStat, applyStatable.BaseValue);
                applyStatable.ApplyValue.Invoke(resultValue);
            }
        }

        public void HandleApplyStat(OnApplyStat applyStat)
        {
            foreach (ApplyStatableStruct applyStatable in applyStatableList)
            {
                float resultValue = _statGetter.GetStat(applyStatable.ApplyStat, applyStatable.BaseValue);
                applyStatable.ApplyValue.Invoke(resultValue);
            }
        }
    }
}