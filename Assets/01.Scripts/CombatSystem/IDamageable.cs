using UnityEngine;

namespace _01.Scripts.CombatSystem
{
    public interface IDamageable
    {
        void ApplyDamage(DamageData damageData, Vector2 hitPoint, Vector2 hitDirection, Vector2 hitNormal);
    }
}