using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _01.Scripts.UI
{
    public class UIPointExit : MonoBehaviour , IPointerExitHandler
    {
        public UnityEvent onExit;
        public void OnPointerExit(PointerEventData eventData) => onExit?.Invoke();
    }
}