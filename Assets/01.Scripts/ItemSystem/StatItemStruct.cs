using System;
using System.Collections.Generic;
using UnityEngine;

namespace _01.Scripts.ItemSystem
{
    [Serializable]
    public struct StatItemStruct
    {
        [field: SerializeField] public StatItemGrade Grade { get; private set; }
        [field: SerializeField] public Color OutLineColor { get; private set; }
        [field: SerializeField] public List<StatItemDataSO> ItemLists { get; private set; }
        [field: SerializeField] public float GachaPer { get; private set; }
    }
}