using _TevLib.Extension.DoT;
using UnityEngine;

namespace _01.Scripts.Agent
{
    public abstract class AbstractBar : MonoBehaviour
    {
        [SerializeField] protected TweenStep tweenStep;
        public abstract void HandleValueChanged(float numerator, float denominator);
    }
}