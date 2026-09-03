using _01.Scripts.ItemSystem;
using _TevLib.CoreLib.EventSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _01.Scripts.GameSystem.Event
{
    public class ItemClickReceiverEvent : GameEvent
    {
        public PointerEventData EventData { get;  private set; }
        public RectTransform RectTrm { get; private set; }
        public ItemClickInvoker ClickInvoker { get; private set; }
        
        public ItemClickReceiverEvent(PointerEventData eventData, RectTransform rectTrm , ItemClickInvoker invoker)
        {
            EventData = eventData;
            ClickInvoker = invoker;
            RectTrm = rectTrm;
        }
    }
}