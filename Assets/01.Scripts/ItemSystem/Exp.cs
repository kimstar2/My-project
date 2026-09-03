using System;
using System.Threading;
using _01.Scripts.GameSystem.Event;
using _01.Scripts.GameSystem.GameServices;
using _TevLib.CoreLib.EventSystem;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.PoolService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.ItemSystem
{
    public class Exp : Collectable
    {
        private EventChannelSO _evtChannel;
        private IPoolingService _poolingService;
        private CancellationTokenSource _cts;
        public UnityEvent onExp;

        private void Start()
        {
            _poolingService = ServiceLocator.GetService<IPoolingService>();
            _evtChannel = ServiceLocator.GetService<IGetEvtChannel>().Evt;
        }

        private void OnEnable()
        {
            onExp?.Invoke();
        }

        public override void Collect(Transform collector, float magneticPower)
        {
            if (!CanCollectable) return;

            AlreadyCollected = true;
            int amount = itemData.GetRandomAmount();

            KillCollectTask();
            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;
            CollectTask(token ,collector, amount, magneticPower).Forget();
        }

        private async UniTask CollectTask(CancellationToken token ,Transform collector, int amount, float magneticPower)
        {
            if (collector == null || magneticPower <= 0f) return;
            Vector3 startPosition = transform.position;

            float distance = Vector2.Distance(startPosition, collector.position);
            float duration = distance / magneticPower;
            float elapsed = 0f;

            try
            {
                while (elapsed < duration)
                {
                    if (collector == null) return;
                    elapsed += Time.deltaTime;
                    float percent = Mathf.Clamp01(elapsed / duration);

                    float easedPercent = percent * percent;

                    transform.position = Vector3.Lerp(
                        startPosition,
                        collector.position,
                        dropCurve.Evaluate(easedPercent)
                    );

                    await UniTask.Yield(cancellationToken: token);
                }
            }
            catch (Exception)
            {
                // ignored
                return;
            }
            
            transform.position = collector.position;

            _evtChannel.Raise(
                new ExpCollectEvent(Collider, amount)
            );

            _poolingService.Push(this);
        }
        
        private void KillCollectTask()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}