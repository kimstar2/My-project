using System;
using System.Collections.Generic;
using UnityEngine;

namespace _TevLib.CoreLib
{
    [Serializable]
    public class NotifyValue<T>
    {
        public delegate void ValueChanged(T prev , T next); // 매개변수 이름 땜시
        public event ValueChanged OnValueChanged;
        
        [SerializeField] private T value;

        public T Value
        {
            get => value;
            set
            {
                T prev = this.value;
                if (EqualityComparer<T>.Default.Equals(prev, value)) return;
                
                this.value = value;
                OnValueChanged?.Invoke(prev, this.value);
            }
        }

        public NotifyValue()
        {
            value = default;
        }
        
        public NotifyValue(T value)
        {
            this.value = value;
        }
    }
}