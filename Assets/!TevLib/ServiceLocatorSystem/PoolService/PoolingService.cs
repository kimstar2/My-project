using System;
using System.Collections.Generic;
using UnityEngine;
using ZLinq;

namespace _TevLib.ServiceLocatorSystem.PoolService
{
    [Serializable]
    public struct PoolItemSetter
    { 
        public Transform parentTrm;
        public PoolItemSO item;
    }
    public class PoolingService : MonoBehaviour , IPoolingService
    {
        [SerializeField] private PoolingListSO poolingList;
        [SerializeField] private List<PoolItemSetter> poolItemSetterList;
        private Dictionary<PoolItemSO, Pool> _poolDict;

        private void Awake()
        {
            ServiceLocator.RegisterService<IPoolingService>(this);
            
            _poolDict = new Dictionary<PoolItemSO, Pool>();

            foreach (PoolItemSetter setter in poolItemSetterList)
                CreatePool(setter);
        }

        private void CreatePool(PoolItemSetter setter)
        {
            IPoolable poolable = setter.item.prefab.GetComponent<IPoolable>();
            Pool pool = new Pool(poolable, setter.parentTrm, setter.item.count);
            _poolDict.Add(setter.item, pool);
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
            Debug.Log("return");
            if (_poolDict.TryGetValue(item.Item, out Pool pool))
                pool.Push(item);
        }

        private void OnDestroy()
        {
            _poolDict.Clear();
            ServiceLocator.UnregisterService<IPoolingService>();
        }

        [ContextMenu("Set")]
        private void Set()
        {
            foreach (PoolItemSO t in poolingList.itemList)
            {
                if (poolItemSetterList.AsValueEnumerable().Select(s => s.item).Contains(t)) continue;
                
                PoolItemSetter setter = new PoolItemSetter
                {
                    item = t,
                    parentTrm = transform
                };
                poolItemSetterList.Add(setter);
            }
        }
    }
}