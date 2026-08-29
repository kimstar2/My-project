using UnityEngine;

namespace _TevLib.Extension.ParticleSystem
{
    public abstract class MonoParticle : MonoBehaviour
    {
        protected UnityEngine.ParticleSystem ParticleSystem;
        protected UnityEngine.ParticleSystem.MainModule Main;
        protected virtual void Awake()
        {
            ParticleSystem = GetComponent<UnityEngine.ParticleSystem>();
            Main = ParticleSystem.main;
        }
    }
}