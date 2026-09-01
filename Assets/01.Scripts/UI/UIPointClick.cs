using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _01.Scripts.UI
{
    public class UIPointClick : MonoBehaviour , IPointerClickHandler
    {
        public UnityEvent onClick;
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }
    }
}