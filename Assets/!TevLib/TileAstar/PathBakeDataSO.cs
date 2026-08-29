using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _TevLib.TileAstar
{
    [CreateAssetMenu(fileName = "Path BakedData", menuName = "TevLib/TileAstar/PathBaked data", order = 0)]
    public class PathBakeDataSO : ScriptableObject
    {
        public List<NodeData> points = new();
        private Dictionary<Vector3Int, NodeData> _pointsDict;

        public void InitializeBakeData()
        {
            if (_pointsDict == null || points.Count != _pointsDict.Count)
                _pointsDict = points.ToDictionary(nodeData => nodeData.cellPos);
        }
        
        public void ClearPoints() => points?.Clear();

        public void AddPoint(Vector3 worldPos, Vector3Int cellPos)
        {
            points.Add(new NodeData(worldPos, cellPos));
        }

        public bool HasNode(Vector3Int cellPos)
        => _pointsDict != null && _pointsDict.ContainsKey(cellPos);

        public bool GetNodeIfExist(Vector3Int cellPos, out NodeData nodeData)
        {
            if (HasNode(cellPos))
            {
                nodeData = _pointsDict[cellPos];
                return true;
            }
            nodeData = default;
            return false;
        }
    }
}