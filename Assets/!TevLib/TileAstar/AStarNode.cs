using System;
using UnityEngine;

namespace _TevLib.TileAstar
{
    public class AStarNode : IComparable<AStarNode>
    {
        public Vector2 WorldPos;
        public Vector3Int CellPos;
        public NodeData NodeData;

        public AStarNode ParentNode;

        public float G;
        public float F; 

        public int CompareTo(AStarNode other)
        {
            if (Mathf.Approximately(other.F, F))
                return 0;
            
            return other.F < F ? -1 : 1;
        }

        public override bool Equals(object obj)
        {
            if (obj is AStarNode node)
                return Equals(node);
            return false;
        }

        public override int GetHashCode() => CellPos.GetHashCode();

        public bool Equals(AStarNode p)
        {
            if (p is null) return false;
            return CellPos == p.CellPos;
        }

        public static bool operator ==(AStarNode lhs, AStarNode rhs)
        {
            if (lhs is null)
            {
                if (rhs is null) return true;
                return false;
            }
            return lhs.Equals(rhs);
        }
        public static bool operator !=(AStarNode lhs, AStarNode rhs) =>  !(lhs == rhs);
    }
}