using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using _TevLib.Editor.PropertyAttribute;
using UnityEngine;

namespace _01.Scripts.Util
{
    
    [Serializable]
    public struct SortingLayerList
    {
        [ReadOnly] public string sortingLayerName;
        public List<OrderInLayerList> orderInLayers;
    }

    [Serializable]
    public struct OrderInLayerList
    {
        public string orderInLayerName;
        public Vector2Int range;
    }
    
    public class CheckSortLayer : MonoBehaviour
    {
        public List<SortingLayerList> sortingLayerList = new();
        
        [ContextMenu("Update List")]
        private void UpdateList()
        {
            foreach (SortingLayer sortingLayer in SortingLayer.layers)
            {
                List<string> d = sortingLayerList.Select(l => l.sortingLayerName).ToList();
                if (d.Contains(sortingLayer.name)) continue;
                SortingLayerList newList = new SortingLayerList {
                    sortingLayerName = sortingLayer.name, };
                
                sortingLayerList.Add(newList);
            }
        }
    }
}
