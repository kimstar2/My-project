using System;
using System.Collections.Generic;
using _TevLib.ModuleSystem;
using UnityEngine;

namespace _01.Scripts.CombatSystem
{
    public abstract class AbstractTickDamageCaster : MonoBehaviour
    {
        [SerializeField] protected int maxHitCount = 5;
        [SerializeField] protected ContactFilter2D contactFilter;
        
        protected Collider2D[] HitResult;
        protected readonly HashSet<IDamageable> DamageableHashSet = new HashSet<IDamageable>();
        protected float CurrentDamage;
        protected Vector2 CurrentDirection;
        protected float CurrentKbForce;
        protected bool IsDamageCasting;
        
        public ModuleOwner Owner { get; private set; }

        public void InitCaster(ModuleOwner owner)
        {
            Owner = owner;
            HitResult = new Collider2D[maxHitCount];
        }

        private void FixedUpdate()
        {
            if (IsDamageCasting)
                ExecuteDamageCast();
        }

        public void OnCastDamage(float damage, Vector2 direction, float kbForce)
        {
            CurrentDamage = damage;
            CurrentDirection = direction;
            CurrentKbForce = kbForce;
            DamageableHashSet.Clear();
            IsDamageCasting = true;
        }

        protected abstract void ExecuteDamageCast();
        


        public void OffCastDamage()
        {
            IsDamageCasting = false;
        }
    }
}