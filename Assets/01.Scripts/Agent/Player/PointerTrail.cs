using System;
using _01.Scripts.GameSystem.Event;
using _TevLib.CoreLib.EventSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _01.Scripts.Agent.Player
{
    [RequireComponent(typeof(TrailRenderer))]
    public class PointerTrail : MonoBehaviour
    {
        [SerializeField] private EventChannelSO eventChannel;
        private PointerPosEvent _pointerPosEvt;
        private TrailRenderer _trailRenderer;

        private void Awake()
        {
            _trailRenderer = GetComponent<TrailRenderer>();
        }

        private void Start() => eventChannel.AddListener<PointerPosEvent>(GetPointerPosEvt);
        
        
        public void OnTrail() => _trailRenderer.enabled = true;
        public void OffTrail() => _trailRenderer.enabled = false;
        
        private void OnDestroy() => eventChannel.RemoveListener<PointerPosEvent>(GetPointerPosEvt);
        private void GetPointerPosEvt(PointerPosEvent pointerPosEvt)
        {
            transform.localPosition = pointerPosEvt.WorldPointerPos;
        }
    }
}