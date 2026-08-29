using _TevLib.Extension.DoT;
using UnityEngine;

namespace _01.Scripts.Agent
{
    public abstract class AbstractHealthBar : MonoBehaviour
    {
        [SerializeField] protected TweenStep tweenStep;
        public abstract void HandleHealthChanged(float health, float maxHealth);
    }
}