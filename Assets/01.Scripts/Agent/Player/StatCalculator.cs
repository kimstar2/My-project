using System.Linq;
using _01.Scripts.ItemSystem;
using _TevLib.ModuleSystem;
using UnityEngine;

namespace _01.Scripts.Agent.Player
{
    public class StatCalculator : MonoModule
    {
        public float CalcStat(StatItemReceiver receiver, float baseValue , StatItemType itemType)
        {
            float resultValue = baseValue;
            foreach (StatItemDataSO itemDataSo in receiver.StatItems)
                resultValue += 
                    itemDataSo.ItemStats.
                        Where(i => i.ItemStatType == itemType).
                        Select(i=>i).
                        Sum(statData => statData.ApplyValue);

            foreach (StatItemDataSO itemDataSo in receiver.StatItems)
            {
                float multipleValue = itemDataSo.ItemStats.Where(i => i.ItemStatType == itemType).Select(i => i)
                    .Sum(statData => statData.Multiple);
                if (Mathf.Approximately(multipleValue, 0f))
                    continue;
                resultValue *= multipleValue;
            }
            
                        
            foreach (StatItemDataSO itemDataSo in receiver.StatItems)
                resultValue += 
                    itemDataSo.ItemStats.
                        Where(i => i.ItemStatType == itemType).
                        Select(i=>i).
                        Sum(statData => statData.FinalApplyValue);
            return resultValue;
        }
    }
}