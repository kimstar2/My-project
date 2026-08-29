using System;
using System.Collections.Generic;

namespace _TevLib.CoreLib
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        public List<T> _heap = new();
        
        public int Count => _heap.Count;
        
        public void Clear() => _heap?.Clear();

        public T Contains(T t)
        {
            int idx = _heap.IndexOf(t);
            if (idx < 0) return default;
            return _heap[idx];
        }

        public void Push(T data)
        {
            _heap.Add(data);
            HeapifyUp(_heap.Count - 1);
        }
        
        private void HeapifyUp(int idx)
        {
            while (idx > 0)
            {
                int parentIdx = (idx - 1) / 2;
                if (_heap[idx].CompareTo(_heap[parentIdx]) <= 0) 
                    break;
                (_heap[idx],_heap[parentIdx]) = (_heap[parentIdx],_heap[idx]);
                idx = parentIdx;
            }
        }
        
        public void DecreaseKey(T item)
        {
            int idx = _heap.IndexOf(item);
            if (idx < 0) return;
            HeapifyUp(idx);
        }

        public T Pop()
        {
            T ret = _heap[0];
            
            int lastIndex = _heap.Count - 1;
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);
            lastIndex--;

            int now = 0;
            while (true)
            {
                int left = 2 * now + 1;
                int right = 2 * now + 2;

                int next = now;
                if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                    next = left;
                if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                    next = right;
                
                if (next == now)
                    break;
                
                (_heap[now] , _heap[next]) = (_heap[next],_heap[now]);

                now = next;
            }
            return ret;
        }

        // 헬퍼 함수
        public T Peek() => _heap.Count == 0 ? default : _heap[0];
    }
}