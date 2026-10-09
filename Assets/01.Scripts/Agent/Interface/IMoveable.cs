using DG.Tweening;
using UnityEngine;

namespace _01.Scripts.Agent.Interface
{
    public interface IMoveable
    {
        public Rigidbody2D RbCompo { get; }
        public Vector2 MoveDirection { get; }
        void SetDirection(Vector2 direction);
        void StopImmediately();
        void Stop();
        void AddForce(Vector2 force ,float ignoreVelocityTime, Ease addForceEase , ForceMode2D forceMode = ForceMode2D.Force);
        void AddForce(Vector2 force ,float ignoreVelocityTime, AnimationCurve addForceEase , ForceMode2D forceMode = ForceMode2D.Force);
    }
}