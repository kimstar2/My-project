using _TevLib.Extension.ParticleSystem;
using UnityEngine;

namespace _01.Scripts.Util
{
    public class MonoParticleEx : MonoParticle
    {
        public void SetLoop(bool loop)
        {
            Main.loop = loop;
        }

        public void SetLoopAndPlay(bool loop)
        {
            SetLoop(loop);
            PlayParticle();
        }

        public void PlayParticle()
        {
            ParticleSystem.Play();
        }

        public void StopParticle()
        {
            ParticleSystem.Stop();
        }
    }
}