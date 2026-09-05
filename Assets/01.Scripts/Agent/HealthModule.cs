using System;
using _TevLib.Editor.PropertyAttribute;
using _TevLib.ModuleSystem;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.Agent
{
    public delegate void HealthChanged(float health, float maxHealth); // 매개변수 이름 암시
    
    public class HealthModule : MonoModule
    {
        [field:SerializeField] public float MaxHealth {get; private set;}
        [field: SerializeField, ReadOnly] public float ReadOnlyHealth { get; private set; }
        
        public UnityEvent<float,float> onHealthChanged;
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
            }
        }
        public bool IsDead { get; private set; }

        public void SetMaxHealth(float value)
        {
            MaxHealth = value;
            Health = Mathf.Clamp(value, 0, MaxHealth);
        }


        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
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