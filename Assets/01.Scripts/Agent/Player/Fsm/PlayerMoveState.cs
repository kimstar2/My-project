using _TevLib.FsmSystem.Runtime;
using UnityEngine;

namespace _01.Scripts.Agent.Player.Fsm
{
    public class PlayerMoveState : ActionablePlayerState
    {
        public PlayerMoveState(IMachineOwner owner, StateSO stateData) : base(owner, stateData)
        {
        }

        protected override bool OnUpdate()
        {
            Vector2 inputDirection = _player.PlayerInput.InputDirection;
            if (inputDirection.sqrMagnitude < MOVE_THRESHOLD)
            {
                _player.ChangeState(PlayerState.IDLE);
                return false;
            }
            
            _player.Mover.SetDirection(inputDirection);
            _player.Renderer.SetMovementDirection(inputDirection);
            
            return true;
        }

        public override void Exit()
        {
            _player.Mover.StopImmediately();
            base.Exit();
        }
    }
}