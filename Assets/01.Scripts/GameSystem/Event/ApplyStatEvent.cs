using _01.Scripts.ItemSystem;
using _TevLib.CoreLib.EventSystem;
using UnityEngine;

namespace _01.Scripts.GameSystem.Event
{
    public class OnApplyStat : GameEvent
    {
        public StatItemType StatItemType { get; private set; }
        public OnApplyStat(StatItemType statItemType) => StatItemType = statItemType;
    }
}