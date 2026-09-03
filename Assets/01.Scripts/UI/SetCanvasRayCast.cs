using System;
using _01.Scripts.GameSystem.GameServices;
using _TevLib.ServiceLocatorSystem;
using UnityEngine;

namespace _01.Scripts.UI
{
    public class SetCanvasRayCast : MonoBehaviour
    {
        private ICanvasBlockRayService _canvasBlockRayService;

        private void Start()
        {
            _canvasBlockRayService = ServiceLocator.GetService<ICanvasBlockRayService>();
        }

        public void SetBlock(bool value)
        {
            _canvasBlockRayService.SetBlock(value);
        }
    }
}