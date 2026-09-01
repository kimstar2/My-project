using System;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.TimeService;
using UnityEngine;

namespace _01.Scripts.Util
{
    public class MonoTimeService : MonoBehaviour
    {
        private ITimeService _timeService;

        private void Start()
        {
            _timeService = ServiceLocator.GetService<ITimeService>();
        }

        public void SetTime(float value)
        {
            _timeService.SetTimeScale(value);
        }
    }
}