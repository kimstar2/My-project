using System;
using System.Collections.Generic;
using UnityEngine;

namespace _TevLib.CoreLib.EventSystem
{
    public abstract class GameEvent { }

    [CreateAssetMenu(fileName = "Event channel", menuName = "TevLib/EventChannel", order = 0)]
    public class EventChannelSO : ScriptableObject
    {
        [SerializeField] private bool isDebug;
        
        private readonly Dictionary<Type, Action<GameEvent>> _events = new(); // Type을 키로 value는 Action<GameEvent> 으로 하는 딕셔너리를 만듦
        private readonly Dictionary<Delegate, Action<GameEvent>> _lookup = new(); // 구독중인 메서드인지 확인하기 위함

        public void AddListener<T>(Action<T> handler) where T : GameEvent 
        {
            if (_lookup.ContainsKey(handler)) return;

            Action<GameEvent> wrappedHandler = e => handler(e as T);
            _lookup[handler] = wrappedHandler;
            
            Type evtType = typeof(T);
            if (!_events.TryAdd(evtType, wrappedHandler)) // 누군가 이미 구독중이라면
            {
                _events[evtType] += wrappedHandler;
            }
        }

        public void RemoveListener<T>(Action<T> handler) where T : GameEvent
        {
            Type evtType = typeof(T);
            if (!_lookup.TryGetValue(handler, out Action<GameEvent> wrappedHandler)) return;

            if (_events.TryGetValue(evtType, out Action<GameEvent> evtHandler))
            {
                evtHandler -= wrappedHandler;
                if (evtHandler == null)
                    _events.Remove(evtType);
                else
                    _events[evtType] = evtHandler;
            }
            _lookup.Remove(handler);
        }

        public void Raise(GameEvent evt)
        {
            if (_events.TryGetValue(evt.GetType(), out Action<GameEvent> evtHandler))
            {
                if (isDebug)
                    Debug.Log($"{evt.GetType().Name} Type => {name} Raise");
                    
                evtHandler?.Invoke(evt);
            }
        }
        
        public void Clear()
        {
            _events.Clear();
            _lookup.Clear();
        }
    }
}