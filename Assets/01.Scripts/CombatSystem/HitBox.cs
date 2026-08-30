using System;
using _01.Scripts.Agent;
using _TevLib.ModuleSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _01.Scripts.CombatSystem
{
    public class HitBox : MonoModule , IDamageable
    {
        private HealthModule _healthModule;
        [Header("HitData")]
        public UnityEvent<Vector3> onHitPoint;
        public UnityEvent<Vector3> onHitDirection;
        public UnityEvent<Vector3> onHitNormal;
        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            _healthModule = owner.GetModule<HealthModule>();
        }

        public void Update() //test
        {
            if (Keyboard.current.tKey.wasPressedThisFrame)
                _healthModule.TakeDamage(-3f);
            
        }

        public void ApplyDamage(DamageData damageData, Vector2 hitPoint, Vector2 hitDirection, Vector2 hitNormal)
        {
            onHitPoint.Invoke(hitPoint);
            onHitDirection.Invoke(hitDirection);
            onHitNormal.Invoke(hitNormal);
            
            _healthModule.TakeDamage(damageData.DamageAmount);
        }
    }
}