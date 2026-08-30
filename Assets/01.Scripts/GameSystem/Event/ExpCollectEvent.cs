using _TevLib.CoreLib.EventSystem;
using UnityEngine;

namespace _01.Scripts.GameSystem.Event
{
    public class ExpCollectEvent : GameEvent
    {
        public Collider2D Collider {get; private set;}
        public int Amount { get; private set;}
        
        public ExpCollectEvent(Collider2D collider, int amount)
        {
            Collider = collider;
            Amount = amount;
        }
    }
}