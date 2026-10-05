using System;
using System.Collections.Generic;
using _TevLib.HashDataSystem;
using _TevLib.ModuleSystem;
using UnityEngine;
using UnityEngine.Events;
using ZLinq;

namespace _01.Scripts.Agent
{
    [Serializable]
    public struct AnimDelegate
    {
        public AnimHashSO animHash;
        public UnityEvent onEvent;
    }
    public class AgentAnimDelegator : MonoModule
    {
        public List<AnimDelegate> delegates;

        private void RaiseAnimEvent(AnimHashSO animHashSO)
        {
            if (animHashSO == null) return;
            int hash = animHashSO.HashValue;
            
            List<UnityEvent> events = delegates
                .AsValueEnumerable()
                .Where(d => d.animHash.HashValue == hash)
                .Select(d => d.onEvent).ToList();
            foreach (UnityEvent e in events)
                e?.Invoke();
        }
    }
}