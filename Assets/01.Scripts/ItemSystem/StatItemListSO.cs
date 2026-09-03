using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _01.Scripts.ItemSystem
{
    [CreateAssetMenu(fileName = "Stat Item List data", menuName = "SO/Item/Stat Item List", order = 0)]
    public class StatItemListSO : ScriptableObject
    {
        [field:SerializeField] public List<StatItemStruct> GachaLists { get; private set; }

        public StatItemDataStruct GetRandomStatItem()
        {
            float sum = GachaLists.Sum(s => s.GachaPer);
            float t = Random.value * sum;
            float acc = 0f;
            StatItemStruct lastValid = default;

            foreach (StatItemStruct group in GachaLists)
            {
                if (group.GachaPer == 0f)
                    continue;

                lastValid = group;
                acc += group.GachaPer;

                if (t < acc)
                {
                    int index = Random.Range(0, group.ItemLists.Count);
                    return new StatItemDataStruct(group.ItemLists[index], group.OutLineColor);
                }
            }
            int lastIndex = Random.Range(0, lastValid.ItemLists.Count);
            return new StatItemDataStruct(lastValid.ItemLists[lastIndex],lastValid.OutLineColor);
        }
    }
    
    public struct StatItemDataStruct
    {
        public StatItemDataSO StatItemDataSO {get; private set;}
        public Color GradeColor {get; private set;}

        public StatItemDataStruct(StatItemDataSO statItemDataSO, Color gradeColor)
        {
            StatItemDataSO = statItemDataSO;
            GradeColor = gradeColor;
        }
    }
}