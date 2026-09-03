using System;
using System.Threading;
using _01.Scripts.GameSystem.Event;
using _01.Scripts.GameSystem.GameServices;
using _TevLib.CoreLib.EventSystem;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.TimeService;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.ItemSystem
{
    public class ItemClickReceiver : MonoBehaviour
    {
        [SerializeField] private EventChannelSO eventChannel;
        [SerializeField] private Transform parentTrm;
        [SerializeField] private float disableTime;
        public UnityEvent<Vector2> onItemAncPos;
        private ItemClickReceiverEvent _currentEvt;
        private Transform _returnTransform;
        private ITimeService _timeService;
        private CancellationTokenSource _returnCts;

        private void Start() => _timeService = ServiceLocator.GetService<ITimeService>();

        private void OnEnable() => eventChannel.AddListener<ItemClickReceiverEvent>(HandleGetItemData);

        private void HandleGetItemData(ItemClickReceiverEvent evt)
        {
            _currentEvt = evt;
            onItemAncPos.Invoke(evt.RectTrm.anchoredPosition);
        }

        public void GetMyHand()
        {
            _returnTransform = _currentEvt.RectTrm.parent.transform;
            _currentEvt.RectTrm.SetParent(parentTrm);
        }

        public void ReturnToParent()
        {
            KillTask();
            _returnCts = new();
            _currentEvt.RectTrm.SetParent(_returnTransform);
            _timeService.ActionTimer(disableTime,
                _returnCts.Token,
                ()=>_currentEvt.ClickInvoker.SetEnable(false),
                ()=>_currentEvt.ClickInvoker.SetEnable(true));
        }

        private void KillTask()
        {
            if (_returnCts != null)
            {
                _returnCts.Cancel();
                _returnCts.Dispose();
                _returnCts = null;
            }
        }
    }
}