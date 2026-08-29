using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.CamTrackingService;
using Unity.Cinemachine;
using UnityEngine;

namespace _01.Scripts.GameSystem.GameServices
{
    public class CameraServiceSetter : MonoBehaviour
    {
        public CinemachineCamera CineCam {get; private set;}
            
        private void Awake()
        {
            CineCam = GetComponent<CinemachineCamera>();
        }

        private void Start()
        {
            CineCam.Target.TrackingTarget = ServiceLocator.GetService<ICamTracking>().TrackingTarget;
        }

        private void Changed()
        {
            CineCam.Target.TrackingTarget = ServiceLocator.GetService<ICamTracking>().TrackingTarget;
        }
    }
}