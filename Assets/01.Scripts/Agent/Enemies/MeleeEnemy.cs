using System;
using _01.Scripts.CombatSystem;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.Agent.Enemies
{
    public class MeleeEnemy : AbstractEnemy
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