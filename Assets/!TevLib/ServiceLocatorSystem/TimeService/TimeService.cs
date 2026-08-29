using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using _TevLib.Editor.PropertyAttribute;
using UnityEngine;

namespace _TevLib.ServiceLocatorSystem.TimeService
{
    public class TimeService : MonoBehaviour, ITimeService, ITickService
    {
        [field: SerializeField, Header("Time System"), ReadOnly] public float GameTime { get; private set; }
        [field: SerializeField , Header("Tick System")] public float MaxTimeTick { get; private set; }
        [field: SerializeField, ReadOnly] public float TimeTick { get; private set; }
        [field: SerializeField, ReadOnly] public int TickCount { get; private set; }

        private float _baseFixedDeltaTime;

        private void Awake()
        {
            ServiceLocator.RegisterService<ITimeService>(this);
            ServiceLocator.RegisterService<ITickService>(this);
            _baseFixedDeltaTime = Time.fixedDeltaTime;
        }

        private void OnDestroy()
        {
            ServiceLocator.UnregisterService<ITimeService>();
            ServiceLocator.UnregisterService<ITickService>();
        }

        private void Update()
        {
            GameTime += Time.deltaTime;
            if (TimeTick < MaxTimeTick)
                TimeTick += Time.deltaTime;
            else
            {
                TimeTick = 0f;
                TickCount++;
            }
        }

        public void SetTimeScale(float timeScale)
        {
            timeScale = Mathf.Max(0f, timeScale);

            Time.timeScale = timeScale;
            Time.fixedDeltaTime = timeScale > 0f
                ? _baseFixedDeltaTime * timeScale
                : _baseFixedDeltaTime;
        }

        public async UniTask Timer(float time, CancellationToken ct) 
            => await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: ct);

        public async UniTask ActionTimer(float time, CancellationToken ct,
            Action startAction = null, Action endAction = null)
        {
            startAction?.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: ct);
            endAction?.Invoke();
        }
    }
}
