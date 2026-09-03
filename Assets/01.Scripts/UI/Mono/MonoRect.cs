using UnityEngine;

namespace _01.Scripts.UI.Mono
{
    public abstract class MonoRect : MonoBehaviour
    {
        protected RectTransform RectTrm { get; private set; }

        protected virtual void Awake()
        {
            RectTrm = GetComponent<RectTransform>();
        }
    }
}