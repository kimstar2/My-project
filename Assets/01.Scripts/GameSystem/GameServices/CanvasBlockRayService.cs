using System;
using _TevLib.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts.GameSystem.GameServices
{
    public class CanvasBlockRayService : MonoBehaviour , ICanvasBlockRayService
    {
        [SerializeField] private Image blockImage;
        public bool IsBlock { get; private set; }
        private void Awake() => ServiceLocator.RegisterService<ICanvasBlockRayService>(this);

        public void SetBlock(bool value)
        {
            IsBlock = value;
            blockImage.raycastTarget = value;
        }
        
        private void OnDestroy()
        {
            ServiceLocator.UnregisterService<ICanvasBlockRayService>();
        }
    }
}