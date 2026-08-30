using System;
using _01.Scripts.GameSystem.Event;
using _01.Scripts.GameSystem.GameServices;
using _TevLib.CoreLib;
using _TevLib.CoreLib.EventSystem;
using _TevLib.Editor.PropertyAttribute;
using _TevLib.ModuleSystem;
using _TevLib.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.Agent.Player
{
    public class PlayerInventory : MonoModule
    {
        [field:SerializeField, ReadOnly] public float ReadOnlyExp {get; private set;}
        [SerializeField] private EventChannelSO eventChannel;
        public UnityEvent<float> expValueChanged;
        [field: SerializeField]
        public NotifyValue<float> Exp { get; private set; } = new();
        
        public void OnEnable()
        {
            Exp.OnValueChanged += HandleExpValueChanged;
            eventChannel.AddListener<ExpCollectEvent>(HandleExpCollect);
        }

        public void ResetExp()
        {
            Exp.Value = 0f;
        }

        private void OnDisable()
        {
            Exp.OnValueChanged -= HandleExpValueChanged;
            eventChannel.RemoveListener<ExpCollectEvent>(HandleExpCollect);
        }

        private void HandleExpValueChanged(float prev, float next)
        {
            ReadOnlyExp = next;
            expValueChanged?.Invoke(ReadOnlyExp);
        }
        private void HandleExpCollect(ExpCollectEvent evt) => Exp.Value += evt.Amount;
    }
}
