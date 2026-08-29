using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _01.Scripts.GameSystem
{
    public enum SkillSlot
    {
        //아직업슴;;;
    }
    [CreateAssetMenu(fileName = "Player Input", menuName = "System/Player Input")]
    public class PlayerInputSO : ScriptableObject , Controls.IPlayerActions
    {
        private Controls _controls;

        public Camera MainCamera { get; private set; }
        public Vector2 InputDirection { get; private set; }
        public Vector2 PointerValue { get; private set; }
        
        public event Action<SkillSlot , bool> OnSkillPerformed;
        public event Action<bool> OnAttackKeyPress;
        public event Action OnInteractKeyPress;
        public event Action OnJumpKeyPress;
        
        public void SetEnable()
        {
            ClearSubscriptions();
            
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
            _controls.Player.Enable();
            
            MainCamera = Camera.main;
        }
        
        private void ClearSubscriptions()
        {
            OnAttackKeyPress = null;
            OnInteractKeyPress = null;
            OnSkillPerformed = null;
            OnJumpKeyPress = null;
        }

        
        public void SetDisable() => _controls?.Player.Disable();
    
        public void OnMove(InputAction.CallbackContext context) => InputDirection =context.ReadValue<Vector2>();

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAttackKeyPress?.Invoke(true);
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnInteractKeyPress?.Invoke();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnJumpKeyPress?.Invoke();
        }

        public void OnPointer(InputAction.CallbackContext context) => PointerValue = context.ReadValue<Vector2>();

        public Vector3 GetPointerToWorldPos()
        {
            Vector3 worldPointerPos = MainCamera!.ScreenToWorldPoint(PointerValue);
            worldPointerPos.z = 0;
            return worldPointerPos;
        }
    }
}
