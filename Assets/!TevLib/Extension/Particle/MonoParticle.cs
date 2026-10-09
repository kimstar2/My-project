using UnityEngine;

namespace _TevLib.Extension.Particle
{
    public abstract class MonoParticle : MonoBehaviour
    {
        protected ParticleSystem ParticleSystem;
        protected ParticleSystem.MainModule Main;
        protected virtual void Awake()
        {
            ParticleSystem = GetComponent<ParticleSystem>();
            Main = ParticleSystem.main;
        }
    }
}