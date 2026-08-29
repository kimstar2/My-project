using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _02.Scenes
{
    public class PlayerInputt : MonoBehaviour
    {
        public delegate void OnJumpEvent();

        public event OnJumpEvent OnJumpKeyPressed;
        public UnityEvent onJumpKeyPressed;
        
        private void OnJump()
        {
            OnJumpKeyPressed?.Invoke();
            onJumpKeyPressed?.Invoke();
        }
    }
}
