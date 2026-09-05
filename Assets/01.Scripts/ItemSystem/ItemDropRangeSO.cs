using UnityEngine;

namespace _01.Scripts.ItemSystem
{
    [CreateAssetMenu(fileName = "Item Drop Range", menuName = "SO/Item/Item Drop Range", order = 0)]
    public class ItemDropRangeSO : ScriptableObject
    {
        [field: Min(0), SerializeField] public int Min { get; private set; }
        [field: Min(0), SerializeField] public int Max { get; private set; }

        public int GetRandomRange()
        {
            int r = Random.Range(Min, Max + 1);
            return r;
        }
    }
}