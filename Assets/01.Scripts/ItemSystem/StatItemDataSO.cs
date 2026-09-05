using System.Collections.Generic;
using UnityEngine;

namespace _01.Scripts.ItemSystem
{
    [CreateAssetMenu(fileName = "Passive Item data", menuName = "SO/Item/Passive Item data", order = 0)]
    public class StatItemDataSO : ScriptableObject
    {
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public List<StatItemData> ItemStats { get; private set; }
        
        public string GetDescContents()
        {
            string result = string.Empty;
            foreach (StatItemData stat in ItemStats)
            {
                string type = GetType(stat);
                string applyValue =  stat.ApplyValue == 0 ? string.Empty :$"{type} {(stat.ApplyValue > 0 ? "+" : "" )}{stat.ApplyValue}\n";
                string multipleValue =  stat.Multiple == 0 ? string.Empty :$"{type} {Mathf.Abs(stat.Multiple-1)*100:F1}% {(stat.Multiple-1 < 0 ? "감소" : "증가" )}\n";
                string finalApplyValue = stat.FinalApplyValue == 0 ? string.Empty : $"최종 {type} {(stat.FinalApplyValue > 0 ? "+" : "" )}{stat.FinalApplyValue}\n";
                result += applyValue + multipleValue + finalApplyValue;
            }
            return result;
        }
        
        private static string GetType(StatItemData stat)
        {
            return stat.ItemStatType switch
            {
                StatItemType.MaxHealth => StatItemTypeVar.MaxHealth,
                StatItemType.Damage => StatItemTypeVar.Damage,
                StatItemType.Speed => StatItemTypeVar.Speed,
                StatItemType.AttackSpeed => StatItemTypeVar.AttackSpeed,
                _ => string.Empty
            };
        }
    }
}