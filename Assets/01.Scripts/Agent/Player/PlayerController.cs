using System;
using _01.Scripts.Agent.Player.Fsm;
using _01.Scripts.GameSystem;
using _TevLib.FsmSystem.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _01.Scripts.Agent.Player
{
    public class PlayerController : AbstractAgent, IMachineOwner
    {
        public GameObject GameObject => gameObject;

        [field: SerializeField] public PlayerInputSO PlayerInput { get; private set; }
        [SerializeField] private StateListSO playerFsmList;

        private StateMachine _stateMachine;
        private RotateObject _weaponHodler;
        public Vector3 WorldPosPointer => PlayerInput.GetPointerToWorldPos();
        
        protected override void InitializeModules()
        {
            base.InitializeModules();

            _stateMachine = new StateMachine(this, playerFsmList.states);
            _weaponHodler = GetModule<RotateObject>();
            PlayerInput.SetEnable();
        }

        protected override void HandleHit()
        {
        }

        private void Start()
        {
            ChangeState(PlayerState.IDLE);
        }

        private void OnDestroy()
        {
            PlayerInput.SetDisable();
        }

        private void Update()
        {
            ObjectDirectionToPointer();
            _stateMachine.UpdateMachine();
        }

        public void ChangeState(PlayerState newState) => _stateMachine.ChangeState((int)newState);
        
        private void ObjectDirectionToPointer()
        {
            Vector2 direction = WorldPosPointer - transform.position;
            if (WorldPosPointer.sqrMagnitude <= Mathf.Epsilon) return;
            float angle =  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _weaponHodler.SetAngle(angle);
        }
    }
}
