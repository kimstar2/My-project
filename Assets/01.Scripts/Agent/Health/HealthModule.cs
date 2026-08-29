using _TevLib.Editor.PropertyAttribute;
using _TevLib.ModuleSystem;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.Agent.Health
{
    public delegate void HealthChanged(float health, float maxHealth); // 매개변수 이름 암시
    
    public class HealthModule : MonoModule
    {
        [field:SerializeField] public float MaxHealth {get; private set;}
        [field: SerializeField, ReadOnly] public float ReadOnlyHealth { get; private set; }
        
        public UnityEvent<float,float> onHealthChanged;
        public UnityEvent<float> onTakeDamage;
        public UnityEvent onDead;
        
        public event HealthChanged OnHealthChanged;
        private float _health;
        public float Health
        {
            get => _health;
            private set
            {
                _health = Mathf.Clamp(value, 0, MaxHealth);
                ReadOnlyHealth = _health;
                OnHealthChanged?.Invoke(_health, MaxHealth);
                onHealthChanged?.Invoke(_health, MaxHealth);
            }
        }
        public bool IsDead { get; private set; }
        
        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            Health = MaxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead) return;
            Health -= damage;
            onTakeDamage.Invoke(damage);
            if (Health <= 0)
            {
                onDead?.Invoke();
                IsDead = true;
            }
        }
    }
}