using _TevLib.ModuleSystem;
using UnityEngine;

namespace _01.Scripts.Agent
{
    public abstract class AbstractWeapon : MonoModule
    {
        public abstract void Attack();
    }
}