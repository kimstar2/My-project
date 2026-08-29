using _01.Scripts.CombatSystem;
using _TevLib.ModuleSystem;
using UnityEngine;

namespace _01.Scripts.Agent
{
    public class RotateObject : MonoModule , IRotatable
    {
        public float ZAngle { get; private set; }

        public void SetAngle(float zAngle)
        {
            ZAngle = zAngle;
            transform.localRotation = Quaternion.Euler(0, 0, ZAngle);
        }
    }
}