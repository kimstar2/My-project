using System;
using _01.Scripts.GameSystem.Event;
using _01.Scripts.GameSystem.GameServices;
using _01.Scripts.UI;
using _TevLib.CoreLib.EventSystem;
using _TevLib.Extension.DoT;
using _TevLib.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _01.Scripts.ItemSystem
{
    public class ItemClickInvoker : MonoRect , IPointerClickHandler
    {
        [SerializeField] private GameObject layoutGroup;
        [SerializeField] private RandomSequencer movingSequencer;
        private EventChannelSO _eventChannel;
        public bool CanClick { get; private set; } = true;
        public bool IsInvoked { get; private set; }
        private Image _image;

        protected override void Awake()
        {
            base.Awake();
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            _eventChannel = ServiceLocator.GetService<IGetEvtChannel>().Evt;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!CanClick) return;
            _eventChannel.Raise(new GetItemDataEvent(eventData , RectTrm , this));
        }

        public void Sequence()
        {
            movingSequencer.Sequence();
            SetEnable(true);
        }
        
        public void SetEnable(bool enable)
        {
            _image.enabled = enable;
            layoutGroup.SetActive(enable);
        }

        public void SetCanClick(bool value) => CanClick = value;
    }
}