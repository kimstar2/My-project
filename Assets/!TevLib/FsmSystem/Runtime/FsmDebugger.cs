using UnityEngine;

namespace _TevLib.FsmSystem.Runtime
{
    public class FsmDebugger : MonoBehaviour
    {
        [field:SerializeField] public bool IsMachineDebug { get; private set; } = false;
        [field:SerializeField] public FsmDebugMessageSO DebugMessageData { get; private set; }
    }
}