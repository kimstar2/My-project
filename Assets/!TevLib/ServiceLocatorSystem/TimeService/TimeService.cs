using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using _TevLib.Editor.PropertyAttribute;
using UnityEngine;
using UnityEngine.Events;

namespace _TevLib.ServiceLocatorSystem.TimeService
{
    public class TimeService : MonoBehaviour, ITimeService, ITickService
    {
        [field: SerializeField, Header("Time System"), ReadOnly] public float GameTime { get; private set; }
        [field: SerializeField , Header("Tick System")] public float SecondTick { get; private set; }
        [field: SerializeField, ReadOnly] public float TimeTick { get; private set; }
        [field: SerializeField, ReadOnly] public int TickCount { get; private set; }

        private float _baseFixedDeltaTime;
        public UnityEvent<int> onHour;
        public UnityEvent<int> onMinute;
        public UnityEvent<int> onSecond;

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
            if (TimeTick < SecondTick)
                TimeTick += Time.deltaTime;
            else
            {
                TimeTick = 0f;
                TickCount++;
                OnSecondTick();
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

        public async UniTask Timer(float time, CancellationToken ct , bool independentTime = false) 
            => await UniTask.Delay(TimeSpan.FromSeconds(time),independentTime, cancellationToken: ct);

        public async UniTask ActionTimer(float time, CancellationToken ct,
            Action startAction = null, Action endAction = null , bool independentTime = false)
        {
            startAction?.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(time),independentTime, cancellationToken: ct);
            endAction?.Invoke();
        }

        # region RealTime
        private int _secondTick;
        private int _minuteTick;
        private int _hourTick;
        private void OnSecondTick()
        {
            _secondTick++;
            if (_secondTick >= 60)
            {
                _secondTick = 0;
                OnMinuteTick();
            }
            onSecond?.Invoke(_secondTick);
        }
        
        private void OnMinuteTick()
        {
            _minuteTick++;
            if (_minuteTick >= 60)
            {
                _minuteTick = 0;
                OnHourTick();
            }
            onMinute?.Invoke(_minuteTick);
        }

        private void OnHourTick()
        {
            _hourTick++;
            if (_hourTick >= 999)
                return;
            onHour?.Invoke(_hourTick);
        }
        #endregion
    }
}
