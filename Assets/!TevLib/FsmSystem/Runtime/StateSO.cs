using _TevLib.HashDataSystem;
using UnityEngine;

namespace _TevLib.FsmSystem.Runtime
{
    [CreateAssetMenu(fileName = "State data", menuName = "TevLib/Fsm/State data", order = 0)]
    public class StateSO : ScriptableObject
    {
        public string stateName;
        public string className;
        public int assetIndex;
        public AnimHashSO animHash;
    }
}