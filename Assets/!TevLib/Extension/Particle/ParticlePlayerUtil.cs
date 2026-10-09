using UnityEngine;

namespace _TevLib.Extension.Particle
{
    public static class ParticlePlayerUtil
    {
        public static void SetParticleDir(ref ParticleSystem par , Vector2 speededDir, ParticleDirType particleDirType)
        {
            switch (particleDirType)
            {
                case ParticleDirType.StartAngle:
                    SetStartAngle(par, speededDir);
                    break;
                case ParticleDirType.Velocity:
                    SetVelocity(par, speededDir);
                    break;
                default:
                    return;
            }
        }

        private static void SetStartAngle(ParticleSystem par, Vector2 dir)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            ParticleSystem.MainModule m = par.main;
            m.startRotation = new ParticleSystem.MinMaxCurve(angle);
        }
        
        private static void SetVelocity(ParticleSystem par, Vector2 speededDir)
        {
            ParticleSystem.VelocityOverLifetimeModule m = par.velocityOverLifetime;
            m.x = speededDir.x;
            m.y = speededDir.y;
        }
    }
}