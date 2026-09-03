using System;
using UnityEngine;

namespace _01.Scripts.ItemSystem
{
    [Serializable]
    public struct StatItemData
    {
        [field: SerializeField] public StatItemType ItemStatType { get; set; }
        [field: SerializeField] public float Multiple { get; set; }
        [field: SerializeField] public float ApplyValue { get; set; }
        [field: SerializeField] public float FinalApplyValue { get; set; }
    }
}