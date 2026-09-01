using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _01.Scripts.UI
{
    public class UIPointEnter : MonoBehaviour , IPointerEnterHandler
    {
        public UnityEvent onEnter;
        public void OnPointerEnter(PointerEventData eventData) => onEnter?.Invoke();
    }
}