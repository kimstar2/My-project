using UnityEngine;

namespace _TevLib.ModuleSystem
{
    public abstract class MonoModule : MonoBehaviour, IInitModule
    {
        public ModuleOwner Owner { get; private set; }
        public virtual void Init(ModuleOwner owner) => Owner = owner;
    }
}