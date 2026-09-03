using System;
using _01.Scripts.GameSystem.Event;
using _01.Scripts.GameSystem.GameServices;
using _01.Scripts.UI;
using _01.Scripts.UI.Mono;
using _TevLib.CoreLib.EventSystem;
using _TevLib.Extension.DoT;
using _TevLib.ServiceLocatorSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _01.Scripts.ItemSystem
{
    public class ItemClickInvoker : MonoRect , IPointerClickHandler
    {
        [field:SerializeField] public StatItemListSO StatItemDataList {get; private set;}
        [SerializeField] private GameObject layoutGroup;
        [SerializeField] private RandomSequencer movingSequencer;
        [Header("Layout Set")]
        [SerializeField] private MonoImage paperImage;
        [SerializeField] private MonoImage iconImage;
        [SerializeField] private MonoTMPUGUI titleText;
        [SerializeField] private MonoTMPUGUI descText;
        [SerializeField] private MonoOutLine outLine;
        private EventChannelSO _eventChannel;
        private StatItemDataSO _crtStatItemData;
        public bool CanClick { get; private set; } = true;
        public bool IsInvoked { get; private set; }

        private void Start()
        {
            _eventChannel = ServiceLocator.GetService<IGetEvtChannel>().Evt;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!CanClick) return;
            _eventChannel.Raise(new ItemClickReceiverEvent(eventData , RectTrm , this));
            _eventChannel.Raise(new ItemDataReceiveEvent(_crtStatItemData));
        }

        public void Sequence()
        {
            movingSequencer.Sequence();
            ValidateStatData();
            SetEnable(true);
        }

        private void ValidateStatData()
        {
            StatItemDataStruct dataStruct = StatItemDataList.GetRandomStatItem();
            _crtStatItemData = dataStruct.StatItemDataSO;
            
            iconImage.SetImage(_crtStatItemData.Icon);
            titleText.SetText(_crtStatItemData.ItemName);
            descText.SetText(_crtStatItemData.GetDescContents());
            outLine.SetColor(dataStruct.GradeColor);
        }

        public void SetEnable(bool enable)
        {
            paperImage.SetEnable(enable);
            outLine.SetEnable(enable);
            layoutGroup.SetActive(enable);
        }

        public void SetCanClick(bool value) => CanClick = value;
    }
}