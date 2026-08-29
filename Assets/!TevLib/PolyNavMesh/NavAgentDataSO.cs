using UnityEngine;

namespace _TevLib.PolyNavMesh
{
    [CreateAssetMenu(fileName = "Nav agent data", menuName = "TevLib/PolyNavMesh/Agent data", order = 10)]
    public class NavAgentDataSO : ScriptableObject
    {
        [field: SerializeField] public float AgentRadius { get; private set; } = 0.5f;
    }
}