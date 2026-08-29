using _01.Scripts.Agent.Interface;
using _TevLib.ModuleSystem;
using UnityEngine;

namespace _01.Scripts.Agent
{
    public abstract class AbstractAgent : ModuleOwner
    {
        public IMoveable Mover {get; private set;}
        public IRenderable Renderer {get; private set;}
        public bool IsDead { get; protected set; }

        protected override void InitializeModules()
        {
            base.InitializeModules();
            
            Mover = GetModule<IMoveable>();
            Debug.Assert(Mover != null,"Mover is null");
            
            Renderer = GetModule<IRenderable>();
            Debug.Assert(Renderer != null, "Renderer is null");
        }
        
        protected abstract void HandleHit();
        protected virtual void HandleDead() => IsDead = true;
        
    }
}