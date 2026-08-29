using System.Collections.Generic;
using UnityEngine;

namespace _TevLib.ServiceLocatorSystem.PoolService
{
    [CreateAssetMenu(fileName = "Pooling List", menuName = "TevLib/Pool/Pooling List", order = 0)]
    public class PoolingListSO : ScriptableObject
    {
        public List<PoolItemSO> itemList;
    }
}