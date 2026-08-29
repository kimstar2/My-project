using UnityEngine;

namespace _01.Scripts.Agent.Interface
{
    public interface IRenderable
    {
        Animator AnimCompo { get; }
        Vector2 FacingDirection { get; }
        void SetMovementDirection(Vector2 direction);
        void RenderClip(int clipHash);
        void RenderClipIfNotPlaying(int clipHash);
    }
}