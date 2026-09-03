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
        [field:SerializeField] public bool IsPlaying {get; private set;}
        [SerializeField] private bool independentTime;
        [SerializeField] private List<WaitAndPlayStep> steps;
        private CancellationTokenSource _cts;

        [ContextMenu("Play")]
        public void Play()
        {
            if (!IsPlaying)
                PlaySteps().Forget();
        }


        private async UniTask PlaySteps()
        {
            IsPlaying = true;
            foreach (var step in steps)
            {
                KillTasks();
                _cts = new CancellationTokenSource();
                step.action.Invoke();
                try
                {
                    await ServiceLocator.GetService<ITimeService>().Timer(step.nextTime,_cts.Token,independentTime);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            IsPlaying = false;
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