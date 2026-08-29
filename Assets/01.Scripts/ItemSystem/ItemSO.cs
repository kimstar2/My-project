using _TevLib.ServiceLocatorSystem.PoolService;
using UnityEngine;

namespace _01.Scripts.ItemSystem
{
    [CreateAssetMenu(fileName = "Item data", menuName = "SO/Item data", order = 0)]
    public class ItemSO : ScriptableObject
    {
        public ItemType itemType;
        public PoolItemSO poolItemSO;
        
        public int minAmount , maxAmount;
        
        public int GetRandomAmount() => Random.Range(minAmount, maxAmount + 1);
    }
}