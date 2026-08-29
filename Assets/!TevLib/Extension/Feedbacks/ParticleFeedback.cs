using _TevLib.Extension.ParticleSystem;
using _TevLib.FeedbackSystem;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.PoolService;
using UnityEngine;

namespace _TevLib.Extension.Feedbacks
{
    public class ParticleFeedback : AbstractFeedback
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
    }
}