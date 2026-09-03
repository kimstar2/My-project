using System.Collections.Generic;
using _01.Scripts.ItemSystem;
using _TevLib.CoreLib.EventSystem;

namespace _01.Scripts.GameSystem.Event
{
    public class ItemDataReceiveEvent : GameEvent
    {
        public StatItemDataSO ItemData { get; private set; } 
        public ItemDataReceiveEvent(StatItemDataSO itemData)
        {
            ItemData = itemData;
        }
    }
}