using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _TevLib.TileAstar
{
    [Serializable]
    public struct NodeData : IEquatable<NodeData>
    {
        public Vector3 worldPos;
        public Vector3Int cellPos;
        public List<LinkData> neighbors;

        public NodeData(Vector3 worldPos, Vector3Int cellPos)
        {
            this.worldPos = worldPos;
            this.cellPos = cellPos;
            neighbors = new();  
        }

        public void AddNeighbor(NodeData neighborNode)
        {
            neighbors.Add(new LinkData
            {
                startPos = worldPos,
                startCellPos = cellPos,
                endPos = neighborNode.worldPos,
                endCellPos = neighborNode.cellPos,
                cost = Vector3Int.Distance(cellPos, neighborNode.cellPos)
            });
        }

        public override int GetHashCode() => cellPos.GetHashCode();
        public bool Equals(NodeData other) => cellPos == other.cellPos;
        
        public override bool Equals(object obj) => obj is NodeData other && Equals(other);

        public static bool operator ==(NodeData lhs, NodeData rhs) => lhs.Equals(rhs);
        public static bool operator !=(NodeData lhs, NodeData rhs) => !(lhs == rhs);
    }
}