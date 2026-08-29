using UnityEngine;

namespace _TevLib.FsmSystem.Runtime
{
    [CreateAssetMenu(fileName = "State list", menuName = "TevLib/Fsm/State list", order = 0)]
    public class StateListSO : ScriptableObject
    {
        [HideInInspector] public string generatePath;
        public string enumName;
        public StateSO[] states;
    }
}