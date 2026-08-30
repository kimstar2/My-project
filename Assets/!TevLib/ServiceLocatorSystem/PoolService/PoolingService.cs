using System;
using System.Collections.Generic;
using UnityEngine;

namespace _TevLib.ServiceLocatorSystem.PoolService
{
    public class PoolingService : MonoBehaviour , IPoolingService
    {
        [SerializeField] private PoolingListSO poolingList;
        private Dictionary<PoolItemSO, Pool> _poolDict;

        private void Awake()
        {
            _poolDict = new Dictionary<PoolItemSO, Pool>();

            foreach (PoolItemSO item in poolingList.itemList)
                CreatePool(item);
            
            ServiceLocator.RegisterService<IPoolingService>(this);
        }

        private void CreatePool(PoolItemSO item)
        {
            IPoolable poolable = item.prefab.GetComponent<IPoolable>();
            Pool pool = new Pool(poolable, transform, item.count);
            _poolDict.Add(item, pool);
        }

        public IPoolable Pop(PoolItemSO itemSo)
        {
            if (!_poolDict.TryGetValue(itemSo, out Pool pool)) return null;
            
            IPoolable item = pool.Pop();
            item.ResetItem();
            return item;
        }

        public void Push(IPoolable item)
        {
            if (_poolDict.TryGetValue(item.Item, out Pool pool))
                pool.Push(item);
        }

        private void OnDestroy()
        {
            _poolDict.Clear();
            ServiceLocator.UnregisterService<IPoolingService>();
        }
    }
}