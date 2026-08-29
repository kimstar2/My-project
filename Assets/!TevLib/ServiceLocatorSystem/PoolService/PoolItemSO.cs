using System;
using UnityEngine;

namespace _TevLib.ServiceLocatorSystem.PoolService
{
    [CreateAssetMenu(fileName = "Pool Item", menuName = "TevLib/Pool/Pool Item", order = 0)]
    public class PoolItemSO : ScriptableObject
    {
        public string itemName;
        public GameObject prefab;
        public int count;

        private void OnValidate()
        {
            if (prefab == null) return;
            
            IPoolable poolable = prefab.GetComponent<IPoolable>();
            
            if (poolable != null) return;
            
            prefab = null;
            Debug.LogWarning("Can not find IPoolable component");
        }
    }
}