using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace _TevLib.ServiceLocatorSystem.TimeService
{
    public interface ITimeService
    {
        float GameTime { get; }
        void SetTimeScale(float timeScale);
        
        UniTask Timer(float time, CancellationToken ct);
        UniTask ActionTimer(float time, CancellationToken ct , Action startAction = null, Action endAction = null);
    }
}