
using UnityEngine;

namespace _TevLib.ServiceLocatorSystem.CamTrackingService
{
    public interface ICamTracking
    {
        Transform TrackingTarget {get;}
        void SetTrackingTarget(Transform trackingTarget);
    }
}