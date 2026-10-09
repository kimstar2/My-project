using System;
using _TevLib.ModuleSystem;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.Agent
{
    
    public class HealthModule : MonoModule
    {
        [field:Header("HealthSet")]
        [field:HorizontalLineAttribute(color: EColor.Gray)]
        [field:SerializeField] public float MaxHealth {get; private set;}
        [field: SerializeField, _TevLib.Editor.PropertyAttribute.ReadOnly] public float ReadOnlyHealth { get; private set; }

        [Header("Events")]
        [HorizontalLineAttribute(color: EColor.Gray)]
        [SerializeField] private bool onRaiseCurrentHealth; 
        public UnityEvent<float,float> onHealthChanged;
        
        [ShowIf("onRaiseCurrentHealth")]
        public UnityEvent<float> onCurrentHealth;
        
        public UnityEvent<float> onTakeDamage;
        public UnityEvent onDead;
        public event Action OnHit;
        
        private float _health;
        public float Health
        {
            get => _health;
            private set
            {
                _health = Mathf.Clamp(value, 0, MaxHealth);
                ReadOnlyHealth = _health;
                onHealthChanged?.Invoke(_health, MaxHealth);
                if (onRaiseCurrentHealth)
                    onCurrentHealth?.Invoke(_health);
            }
        }
        public bool IsDead { get; private set; }

        public void SetMaxHealth(float value)
        {
            MaxHealth = value;
            _ = Health;
        }

        private void Start()
        {
            HealthInit();
        }

        public void HealthInit()
        {
            Health = MaxHealth;
            IsDead = false;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead) return;
            Health -= damage;
            
            onTakeDamage?.Invoke(damage);
            OnHit?.Invoke();
            
            if (Health <= 0)
            {
                onDead?.Invoke();
                IsDead = true;
            }
        }

        #region Helper

        public float GetHealthPer() => Health / MaxHealth;

        #endregion
    }
}