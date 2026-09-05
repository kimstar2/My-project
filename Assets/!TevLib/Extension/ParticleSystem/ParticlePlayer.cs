using System.Threading;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.PoolService;
using _TevLib.ServiceLocatorSystem.TimeService;
using UnityEngine;

namespace _TevLib.Extension.ParticleSystem
{
    public class ParticlePlayer : MonoParticle , IPoolable
    {
        [field:SerializeField] public PoolItemSO Item { get; private set; }
        public GameObject GameObject => gameObject;
        private float _duration;
        private CancellationTokenSource _cts;

        protected override void Awake()
        {
            base.Awake();
            _duration = Main.duration;
        }

        public void SetPositionAndPlay(Vector3 position)
        {
            KillToken();
            transform.position = position;
            _cts = new CancellationTokenSource();
            ServiceLocator.GetService<ITimeService>().ActionTimer(
                _duration,
                _cts.Token,
            ParticlePlay,
                ReturnGoToPool
                );
        }

        private void ParticlePlay() => ParticleSystem.Play();
        public void ParticleStop()
        {
            KillToken();
            ParticleSystem.Stop();
            ParticleSystem.Simulate(0);
        }
        
        # region Pool
        public void ResetItem() => ParticleStop();

        public void ReturnGoToPool()
        {
            ServiceLocator.GetService<IPoolingService>()?.Push(this);
        }
        
        # endregion
        # region UniTask
        private void KillToken()
        {
            if (_cts == null) return;
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
        # endregion
    }
}