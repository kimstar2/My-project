using _TevLib.CoreLib.EventSystem;
using UnityEngine;

namespace _01.Scripts.GameSystem.Event
{
    public class PointerPosEvent : GameEvent
    {
        public Vector3 PointerPos { get; private set; }
        public Vector3 WorldPointerPos { get; private set; }
        
        public void SetPointerPos(Vector3 pointerPos) => PointerPos = pointerPos;
        public void SetPointerToWorldPos(Vector3 worldPointerPos)
        {
            WorldPointerPos = worldPointerPos;
        }

    }
}