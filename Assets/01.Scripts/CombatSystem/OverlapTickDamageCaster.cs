using System.Collections.Generic;
using UnityEngine;

namespace _01.Scripts.CombatSystem
{
    public class OverlapTickDamageCaster : AbstractTickDamageCaster
    {
        public enum CastType
        {
            Circle,Box
        }
        
        [SerializeField] private CastType castType;
        [SerializeField] private float radius; // 원형 캐스트 전용
        [SerializeField] private Vector2 boxSize; // 박스 캐스트 전용
        public bool IsTick { get; private set; } = false;

        public void SetRadius(float value) => radius = value; 
        public void SetBoxSize(Vector2 value) => boxSize = value;
        
        protected override void ExecuteDamageCast()
        {
            int cnt = castType switch
            {
                CastType.Circle => Physics2D.OverlapCircle(transform.position, radius, contactFilter, HitResult),
                CastType.Box => Physics2D.OverlapBox(transform.position, boxSize, 0, contactFilter, HitResult),
                _ => 0
            };

            for (int i = 0; i < cnt; i++)
            {
                if (!HitResult[i].TryGetComponent(out IDamageable damageable)) continue;
                if (!DamageableHashSet.Add(damageable)) continue;
                                
                Vector2 point = HitResult[i].ClosestPoint(transform.position); // 가장 가까운 포인트
                Vector2 knockbackForce = CurrentDirection.normalized * CurrentKbForce;
                DamageData damageData = new DamageData
                {
                    DamageAmount = CurrentDamage,
                    Dealer = Owner,
                    DirectedKBForce = knockbackForce
                };
                
                damageable.ApplyDamage(damageData,point,CurrentDirection,-CurrentDirection);
            }
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (castType == CastType.Circle)
                Gizmos.DrawWireSphere(transform.position, radius);
            else if  (castType == CastType.Box)
                Gizmos.DrawWireCube(transform.position, boxSize);
        }
        #endif
    }
}