using System;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.Util
{
    public class ToggleEvent : MonoBehaviour
    {
        [SerializeField] private bool defaultToggleChecked;
        public bool CurrentToggleValue { get; private set; }
        public UnityEvent onToggleChecked;
        public UnityEvent onToggleUnchecked;
        
        private void Awake() => CurrentToggleValue = defaultToggleChecked;

        public void Toggle()
        {
            CurrentToggleValue = !CurrentToggleValue;
            if (CurrentToggleValue)
                onToggleChecked.Invoke();
            else
                onToggleUnchecked.Invoke();
        }
    }
}