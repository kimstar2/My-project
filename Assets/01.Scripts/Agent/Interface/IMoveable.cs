using UnityEngine;

namespace _01.Scripts.Agent.Interface
{
    public interface IMoveable
    {
        public Rigidbody2D RbCompo { get; }
        public Vector2 MoveDirection { get; }
        void SetDirection(Vector2 direction);
        void StopImmediately();
    }
}