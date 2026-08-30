using System;
using _TevLib.CoreLib.EventSystem;
using _TevLib.ServiceLocatorSystem;
using UnityEngine;

namespace _01.Scripts.GameSystem.GameServices
{
    public class GetEvtService : MonoBehaviour , IGetEvtChannel
    {
        [SerializeField] private EventChannelSO evtChannel;

        private void Awake() => ServiceLocator.RegisterService<IGetEvtChannel>(this);

        private void OnDestroy() => ServiceLocator.UnregisterService<IGetEvtChannel>();
        public EventChannelSO Evt => evtChannel;
    }
}