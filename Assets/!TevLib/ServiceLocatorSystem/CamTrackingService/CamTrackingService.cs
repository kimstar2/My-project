using UnityEngine;

namespace _TevLib.ServiceLocatorSystem.CamTrackingService
{
    public class CamTrackingService : MonoBehaviour , ICamTracking
    {
        [field: SerializeField] public Transform TrackingTarget {get; private set;}
        private const string CamTrackingName = " [ Cam Tracking ]";
        private string _memorizeName;

        private void Awake()
        {
            ServiceLocator.RegisterService<ICamTracking>(this);
            SetName();
        }

        private void OnDestroy() 
            => ServiceLocator.UnregisterService<ICamTracking>();
        
        public void SetTrackingTarget(Transform trackingTarget)
        {
            UnSetName();
            TrackingTarget = trackingTarget;
            SetName();
        }
        
        [ContextMenu("Change Target")]
        public void SetTrackingTarget()
        {
            UnSetName();
            SetName();
        }
        
        
        private void UnSetName()
            => TrackingTarget.name = _memorizeName;

        private void SetName()
        {
            _memorizeName = TrackingTarget.name;
            TrackingTarget.name += CamTrackingName;
        }
    }
}