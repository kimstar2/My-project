using _TevLib.Extension.Particle;
using _TevLib.FeedbackSystem;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.PoolService;
using UnityEngine;

namespace _TevLib.Extension.Feedbacks
{
    public class ParticleFeedback : AbstractFeedback , IFeedbackDir
    {
        [SerializeField] private PoolItemSO particleItemSo;
        [SerializeField] private Transform defaultTrm;
        private ParticlePlayer _crtParticlePlayer;

        public override void PlayFeedback()
        {
            _crtParticlePlayer = ServiceLocator.GetService<IPoolingService>().Pop(particleItemSo) as ParticlePlayer;
            _crtParticlePlayer?.SetPositionAndPlay(defaultTrm.position);
        }

        public override void StopFeedback()
        {
            if (_crtParticlePlayer == null) return;
            _crtParticlePlayer.ParticleStop();
            _crtParticlePlayer.ReturnGoToPool();
        }

        public void SetPosAndPlay(Vector3 position)
        {
            _crtParticlePlayer = ServiceLocator.GetService<IPoolingService>().Pop(particleItemSo) as ParticlePlayer;
            _crtParticlePlayer?.SetPositionAndPlay(position);
        }

        public void SetDirAndPlay(Vector2 dir)
        {
            _crtParticlePlayer = ServiceLocator.GetService<IPoolingService>().Pop(particleItemSo) as ParticlePlayer;
            _crtParticlePlayer?.SetDir(dir);
            _crtParticlePlayer?.SetPositionAndPlay(defaultTrm.position);
        }
    }
}