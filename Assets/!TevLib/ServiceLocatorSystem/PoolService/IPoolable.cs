using UnityEngine;

namespace _TevLib.ServiceLocatorSystem.PoolService
{
    public interface IPoolable
    {
        public PoolItemSO Item { get; }
        public GameObject GameObject { get; }

        public void ResetItem();
    }
}