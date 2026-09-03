using System;
using System.Collections.Generic;
using _01.Scripts.GameSystem.Event;
using _01.Scripts.ItemSystem;
using _TevLib.CoreLib.EventSystem;
using _TevLib.ModuleSystem;
using UnityEngine;

namespace _01.Scripts.Agent.Player
{
    public class StatItemReceiver : MonoModule
    {
        [field: SerializeField] public List<StatItemDataSO> StatItems { get; private set; }
        [SerializeField] private EventChannelSO eventChannel;
        
        private void OnEnable() => eventChannel.AddListener<ItemDataReceiveEvent>(HandleItemDataReceive);
        private void OnDisable() => eventChannel.RemoveListener<ItemDataReceiveEvent>(HandleItemDataReceive);
        private void HandleItemDataReceive(ItemDataReceiveEvent itemData)
        {
            StatItems.Add(itemData.ItemData);

            foreach (StatItemData itemStatData in itemData.ItemData.ItemStats)
                eventChannel.Raise(new OnApplyStat(itemStatData.ItemStatType));
        }
    }
}