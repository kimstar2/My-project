using _01.Scripts.CombatSystem;
using UnityEngine;

namespace _01.Scripts.Agent.Enemies.Abs
{
    public abstract class RotatableEnemy : AbstractEnemy
    {
        private IRotatable _rotatable;
        
        protected override void InitializeModules()
        {
            base.InitializeModules();
            _rotatable = GetModule<IRotatable>();
        }

        private void LateUpdate()
        {
            RotationHodler();
        }

        private void RotationHodler()
        {
            Vector2 aimDir = Renderer.FacingDirection;
            float angle = Mathf.Atan2(aimDir.y,aimDir.x) * Mathf.Rad2Deg;
            _rotatable.SetAngle(angle);
        }
    }
}