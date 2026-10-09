using _01.Scripts.Agent;
using _TevLib.ModuleSystem;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace _01.Scripts.CombatSystem
{
    public class KnockbackableHitbox : HitBox
    {
        [Header("Knockback Settings")] 
        [HorizontalLine(color: EColor.Gray)]
        [SerializeField] private Ease knockbackEase;
        [SerializeField] private float knockbackResistance;
        [SerializeField] private float knockbackTime;
        private AgentMovement _agentMover;
        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            _agentMover = owner.GetModule<AgentMovement>();
            Debug.Assert(_agentMover != null,"[KnockbackableHitbox] AgentMover is Null");
        }

        public override void ApplyDamage(DamageData damageData, Vector2 hitPoint, Vector2 hitDirection, Vector2 hitNormal)
        {
            base.ApplyDamage(damageData, hitPoint, hitDirection, hitNormal);
            Vector2 calcedKb = damageData.DirectedKBForce - Vector2.one * knockbackResistance;
            _agentMover.AddForce(calcedKb, knockbackTime, knockbackEase, ForceMode2D.Impulse);
        }
    }
}