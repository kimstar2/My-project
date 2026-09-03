using System;
using _01.Scripts.GameSystem.Event;
using _01.Scripts.GameSystem.GameServices;
using _01.Scripts.ItemSystem;
using _TevLib.CoreLib.EventSystem;
using _TevLib.ModuleSystem;
using _TevLib.ServiceLocatorSystem;
using UnityEngine;

namespace _01.Scripts.Agent.Player
{
    public class PlayerStatGetter : MonoModule
    {
        [SerializeField] private EventChannelSO eventChannel;
        public StatItemReceiver StatItemReceiver {get; private set;}
        public StatCalculator StatCalculator {get; private set;}
        
        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            StatItemReceiver = owner.GetModule<StatItemReceiver>();
            StatCalculator = owner.GetModule<StatCalculator>();
        }
        
        public float GetStat(StatItemType statItemType , float baseValue)
        {
            float resultValue = StatCalculator.CalcStat(StatItemReceiver , baseValue, statItemType);
            return resultValue;
        }
    }
}