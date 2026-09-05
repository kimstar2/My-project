using System;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.PoolService;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01.Scripts.ItemSystem
{
    public class DropItem : MonoBehaviour
    {
        [SerializeField] private DropTableSO dropTable;
        private IPoolingService _poolingService;

        private void Start()
        {
            _poolingService = ServiceLocator.GetService<IPoolingService>();
        }

        public void DropIt()
        {
            foreach (DropInfo dropInfo in dropTable.dropTable)
                Drop(dropInfo);
        }
        
        public void DropIt(ItemDropRangeSO dropRange)
        {
            for (int i = 0; i < dropRange.GetRandomRange(); i++)
            {
                foreach (DropInfo dropInfo in dropTable.dropTable)
                    Drop(dropInfo);
            }
        }

        private void Drop(DropInfo dropInfo)
        {
            if (dropInfo.dropRate > Random.value)
            {
                IPoolable item = _poolingService.Pop(dropInfo.item.poolItemSO);
                Collectable itemInfo = item.GameObject.GetComponent<Collectable>();
                itemInfo.SetItemData(dropInfo.item);
                Vector3 dropDir = Quaternion.Euler(0, 0, Random.Range(-50f, 50f)) * Vector3.up;
                itemInfo.DropIt(transform.position + dropDir);
            }
        }
    }
}