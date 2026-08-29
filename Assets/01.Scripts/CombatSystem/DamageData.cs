using _TevLib.ModuleSystem;
using UnityEngine;

namespace _01.Scripts.CombatSystem
{
    public struct DamageData
    {
        public float DamageAmount;
        public bool IsCritical;
        public ModuleOwner Dealer;
        public Vector2 DirectedKBForce;
    }
}