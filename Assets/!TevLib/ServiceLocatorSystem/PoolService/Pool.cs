using System.Collections.Generic;
using UnityEngine;

namespace _TevLib.ServiceLocatorSystem.PoolService
{
    public class Pool
    {
        private readonly Stack<IPoolable> _pool;
        private readonly Transform _parentTrm;
        private readonly IPoolable _poolable;
        private readonly GameObject _prefab;
        
        public Pool(IPoolable poolable ,Transform parentTrm , int count)
        {
            _pool = new Stack<IPoolable>(count);
            _parentTrm = parentTrm;
            _poolable = poolable;
            _prefab = poolable.GameObject;
            
            for (int i = 0; i < count; i++)
            {
                IPoolable item = CreatePoolItem();
                _pool.Push(item);
            }
        }

        private IPoolable CreatePoolItem()
        {
            GameObject poolGo = Object.Instantiate(_prefab, _parentTrm);
            poolGo.SetActive(false);
            poolGo.name = _poolable.Item.itemName;
            return  poolGo.GetComponent<IPoolable>();
        }

        public IPoolable Pop()
        {
            IPoolable item;
            if (_pool.Count <= 0)
                item = CreatePoolItem();
            else
                item = _pool.Pop();
            item.GameObject.SetActive(true);
            
            return item;
        }

        public void Push(IPoolable item)
        {
            item.GameObject.SetActive(false);
            _pool.Push(item);
        }
    }
}