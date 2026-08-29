using System;
using UnityEngine;

namespace _TevLib.FsmSystem.Runtime
{
    [CreateAssetMenu(fileName = "FsmDebug message", menuName = "TevLib/Fsm/Debug message", order = 0)]
    public class FsmDebugMessageSO : ScriptableObject
    {
        [field: SerializeField] public bool NewState { get; private set; }
        [field: SerializeField] public bool CurrentState { get; private set; }
        
        [field: SerializeField,Tooltip("First => {0} , Second => {1}")]
        public string DebugMessage { get; private set; }
    }
}