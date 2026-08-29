using System;
using _TevLib.CustomUtility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _TevLib.TileAstar
{
    public class PathBaker : MonoBehaviour
    {
        [SerializeField] private Tilemap groundMap;
        [SerializeField] private Tilemap obstacleMap;
        [SerializeField] private PathBakeDataSO bakedData;

        [SerializeField] private bool isDrawGizmo = true;
        [SerializeField] private bool isCornerCheck = true;
        [SerializeField] private Color nodeColor , edgeColor;

        [ContextMenu("Bake Map")]
        private void BakeMap()
        {
            if (groundMap == null || obstacleMap == null || bakedData == null)
            {
                Debug.Log("[PathBaker] RequireData is null]");
                return;
            }
            WritePointData();
            RecordNeighbors();
            WriteIfUnityEditor();
        }

        private void Awake()
        {
            bakedData?.InitializeBakeData();
        }

        private void WritePointData()
        {
            bakedData.ClearPoints();
            groundMap.CompressBounds();
            
            BoundsInt mapBounds = groundMap.cellBounds;

            for (int x = mapBounds.xMin; x < mapBounds.xMax; x++)
            {
                for (int y = mapBounds.yMin; y < mapBounds.yMax; y++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);
                    if (CanMovePosition(cellPos))
                        AddPoint(cellPos);
                }
            }
            
            bakedData.InitializeBakeData();
        }

        private void AddPoint(Vector3Int cellPos)
        {
            Vector3 worldPos = groundMap.GetCellCenterWorld(cellPos);
            bakedData.AddPoint(worldPos,cellPos);
        }

        private bool CanMovePosition(Vector3Int cellPos)
        {
            bool hasObstacle = obstacleMap.HasTile(cellPos);
            bool hasGround = groundMap.HasTile(cellPos);
            
            return !hasObstacle && hasGround;
        }
        
        private void RecordNeighbors()
        {
            foreach (NodeData node in bakedData.points)
            {
                node.neighbors.Clear();

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0) continue;

                        Vector3Int nextPoint = new Vector3Int(x, y) + node.cellPos;
                        if (bakedData.GetNodeIfExist(nextPoint, out NodeData adjacentNode))
                        {
                            if (CheckCorner(nextPoint, node.cellPos))
                                node.AddNeighbor(adjacentNode);
                        }
                    }
                }
            }
        }

        private bool CheckCorner(Vector3Int nextPoint, Vector3Int currentPoint)
        {
            if (!isCornerCheck) return true;
            
            return CanMovePosition(new Vector3Int(nextPoint.x,currentPoint.y))
                && CanMovePosition(new Vector3Int(currentPoint.x,nextPoint.y));
        }

        private void WriteIfUnityEditor()
        {
            #if UNITY_EDITOR
            EditorUtility.SetDirty(bakedData);
            AssetDatabase.SaveAssets();
            #endif
        }

        private void OnDrawGizmosSelected()
        {
            if (!isDrawGizmo) return;
            if (bakedData == null) return;

            foreach (NodeData node in bakedData.points)
            {
                Gizmos.color = nodeColor;
                Gizmos.DrawWireSphere(node.worldPos,0.2f);

                foreach (LinkData link in node.neighbors)
                {
                    Gizmos.color = edgeColor;
                    DrawArrowGizmo(link.startPos,link.endPos);
                }
            }
        }
        public  void DrawArrowGizmo(Vector3 start, Vector3 end)
        {
            Vector3 dir = end - start;
            
            Vector3 normalDir = dir.normalized;
            Vector3 arrowStart = end - normalDir * 0.25f;
            Vector3 arrowEnd = end - normalDir * 0.15f;
            const float arrowSize = 0.05f;
            
            Vector3 triangleA = arrowStart + (Quaternion.Euler(0,0,-90f) * normalDir) * arrowSize;
            Vector3 triangleB = arrowStart + (Quaternion.Euler(0,0,90f) * normalDir) * arrowSize;
            
            Gizmos.DrawLine(start, arrowEnd);
            Gizmos.DrawLine(triangleA, arrowEnd);
            Gizmos.DrawLine(triangleB, arrowEnd);
            Gizmos.DrawLine(triangleA, triangleB);
        }
    }
}