using System;
using System.Collections.Generic;
using System.Threading;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.TimeService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.Util
{
    [Serializable]
    public struct WaitAndPlayStep
    {
        public UnityEvent action;
        public float nextTime;
    }
    
    public class WaitAndPlay : MonoBehaviour
    {
        [SerializeField] private List<WaitAndPlayStep> steps;
        private CancellationTokenSource _cts;

        public void Play() => PlaySteps().Forget();
        
        public async UniTask PlaySteps()
        {
            foreach (var step in steps)
            {
                KillTasks();
                _cts = new CancellationTokenSource();
                step.action.Invoke();
                await ServiceLocator.GetService<ITimeService>().Timer(step.nextTime,_cts.Token);
            }
        }
        
        private void KillTasks()
        {
            if (_cts == null) return;
            
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }
}