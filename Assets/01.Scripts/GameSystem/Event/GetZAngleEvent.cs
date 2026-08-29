using _TevLib.CoreLib.EventSystem;

namespace _01.Scripts.GameSystem.Event
{
    public class GetZAngleEvent : GameEvent
    {
        public float ZAngle {get; private set;}
        public GetZAngleEvent(float zAngle)
        {
            ZAngle = zAngle;
        }
    }
}