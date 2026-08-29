using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _TevLib.CoreLib;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _TevLib.TileAstar
{
    public class PathAgent : MonoBehaviour
    {
        [SerializeField] private PathBakeDataSO bakedData;

        private CancellationTokenSource _cts = new();
        private UniTask<(List<AStarNode> nodes, bool isSuccess)>? _calculatingTask; // 경로 계산 Task
        private bool _isCalculating;
        private Vector3Int _lastDestination;

        private readonly Stack<AStarNode> _nodePool = new();
        private readonly List<AStarNode> _rentedNodes = new();

        public bool PathPending => _isCalculating;
        public bool HasPath { get; private set; }
        public bool IsPathStale { get; private set; }

        private (List<AStarNode> nodes, bool isSuccess) CalculatePath(
            Vector3Int startPosition, Vector3Int destination, CancellationToken ct)
        {
            foreach (AStarNode node in _rentedNodes)
            {
                node.ParentNode = null; // 참조 순환을 막기위해
                _nodePool.Push(node);
            }
            _rentedNodes.Clear();
            
            //초기화
            PriorityQueue<AStarNode> openList = new();
            HashSet<Vector3Int> closedSet = new();
            List<AStarNode> path = new();
            AStarNode foundNode = null;
            
            if (bakedData.GetNodeIfExist(startPosition, out NodeData startNode) == false)
                return (path, false);
            if (bakedData.GetNodeIfExist(destination, out NodeData destNode) == false)
                return (path, false);

            AStarNode startAstar = Rent(); // 시작 노드 발행후 변경
            startAstar.NodeData = startNode;
            startAstar.CellPos = startNode.cellPos;
            startAstar.WorldPos = startNode.worldPos;
            startAstar.ParentNode = null;
            startAstar.G = 0;
            startAstar.F = CalcH(startNode.cellPos, destNode.cellPos);
            openList.Push(startAstar);

            while (openList.Count > 0)
            {
                if (ct.IsCancellationRequested)
                    break;
                
                AStarNode currentNode = openList.Pop();
                
                if (!closedSet.Add(currentNode.CellPos))
                    continue;
                if (currentNode.NodeData == destNode)
                {
                    foundNode = currentNode;
                    break;
                }

                foreach (LinkData link in currentNode.NodeData.neighbors)
                {
                    if (closedSet.Contains(link.endCellPos)) continue;
                    
                    if (bakedData.GetNodeIfExist(link.endCellPos, out NodeData nextNode) == false)
                        continue;

                    float newG = link.cost + currentNode.G;

                    AStarNode nextAstar = Rent();
                    nextAstar.NodeData = nextNode;
                    nextAstar.CellPos = nextNode.cellPos;
                    nextAstar.WorldPos = nextNode.worldPos;
                    nextAstar.ParentNode = currentNode;
                    nextAstar.G = newG;
                    nextAstar.F = newG + CalcH(nextNode.cellPos, destNode.cellPos);
                    
                    // 이미 오픈 클래스에 있다면 어떤 값이 작은지 체크
                    AStarNode existInOpenNode = openList.Contains(nextAstar);
                    if (existInOpenNode != null)
                    {
                        if (nextAstar.G < existInOpenNode.G)
                        {
                            existInOpenNode.G = nextAstar.G;
                            existInOpenNode.F = nextAstar.F;
                            existInOpenNode.ParentNode = nextAstar.ParentNode;
                            openList.DecreaseKey(existInOpenNode); // 우선순위 갱신
                        }

                        ReturnLast(); // 현재 Astar 노드는 사용 되지 않으니 반납
                    }
                    else 
                        openList.Push(nextAstar);
                }
            }

            if (foundNode != null)
            {
                AStarNode node = foundNode;
                while (node != null)
                {
                    path.Add(node);
                    node = node.ParentNode;
                }
                path.Reverse();
            }
            return (path, foundNode != null);
        }

        public void CancelPath()
        {
            if (_isCalculating)
                _cts?.Cancel();
        }

        public async UniTask<int> GetPath(Vector3Int startPos, Vector3Int destination, Vector3[] pointArr)
        {
            if (_isCalculating)
            {
                _cts?.Cancel();
                if (_calculatingTask.HasValue)
                {
                    try
                    {
                        await _calculatingTask.Value.AsValueTask().ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) {}
                }
            }
            
            if (HasPath && destination != _lastDestination)
                IsPathStale = true;
            
            // null 이거나 이미 취소된 경우 새로 생성
            if (_cts is null or { IsCancellationRequested: true} )
                _cts = new CancellationTokenSource();
            
            CancellationToken ct = _cts.Token;

            try
            {
                _isCalculating = true;
                _calculatingTask = Task.Run(() => CalculatePath(startPos, destination, ct), ct).AsUniTask();
                (List<AStarNode> list, bool isSuccess) = await _calculatingTask.Value; // 작업 종료 대기

                if (ct.IsCancellationRequested)
                    return 0;

                int cornerIndex = 0;

                if (isSuccess)
                {
                    pointArr[cornerIndex] = list[0].WorldPos; // 시작점
                    cornerIndex++;

                    for (int i = 1; i < list.Count - 1; i++)
                    {
                        if (cornerIndex >= pointArr.Length) break;

                        Vector3Int beforeDir = list[i].CellPos - list[i - 1].CellPos;
                        Vector3Int afterDir = list[i + 1].CellPos - list[i].CellPos;
                        if (beforeDir != afterDir)
                        {
                            pointArr[cornerIndex] = list[i].WorldPos;
                            cornerIndex++;
                        }
                    }

                    if (list.Count > 1 && cornerIndex < pointArr.Length)
                    {
                        pointArr[cornerIndex] = list[^1].WorldPos;
                        cornerIndex++;
                    }

                    HasPath = true;
                    IsPathStale = false;
                    _lastDestination = destination;
                }
                else
                {
                    HasPath = false;
                    IsPathStale = false;
                }

                return cornerIndex;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                HasPath = false;
                IsPathStale = false;
                return -1;
            }
            finally
            {
                _isCalculating = false;
            }
        }

        #region Helper Methods
        
        private AStarNode Rent()
        {
            AStarNode node = _nodePool.Count > 0 ? _nodePool.Pop() : new AStarNode();
            _rentedNodes.Add(node);
            return node;
        }

        private void ReturnLast()
        {
            int last = _rentedNodes.Count - 1;
            AStarNode node = _rentedNodes[last];
            _rentedNodes.RemoveAt(last);
            node.ParentNode = null;
            _nodePool.Push(node);
        }
        
        private float CalcH(Vector3Int startPoint, Vector3Int destPoint) 
            => Vector3Int.Distance(startPoint, destPoint);
        #endregion
    }
}