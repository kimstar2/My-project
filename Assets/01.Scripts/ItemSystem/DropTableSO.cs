using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace _01.Scripts.ItemSystem
{
    [Serializable]
    public struct DropInfo
    {
        public ItemSO item;
        public float dropRate;
    }
    
    
    [CreateAssetMenu(fileName = "DropTable", menuName = "SO/Item/DropTable", order = 0)]
    public class DropTableSO : ScriptableObject
    {
        public List<DropInfo> dropTable;
    }
}