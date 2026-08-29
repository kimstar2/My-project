using System.Collections.Generic;

namespace _TevLib.CoreLib
{
    public class NotifyValue<T>
    {
        public delegate void ValueChanged(T prev , T next); // 매개변수 이름 땜시
        public event ValueChanged OnValueChanged;
        
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                T prev = _value;
                if (EqualityComparer<T>.Default.Equals(prev, value)) return;
                
                _value = value;
                OnValueChanged?.Invoke(prev, _value);
            }
        }

        public NotifyValue()
        {
            _value = default;
        }
        
        public NotifyValue(T value)
        {
            _value = value;
        }
    }
}