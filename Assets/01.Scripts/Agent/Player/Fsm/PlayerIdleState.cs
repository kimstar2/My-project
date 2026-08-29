using _TevLib.FsmSystem.Runtime;
using UnityEngine;

namespace _01.Scripts.Agent.Player.Fsm
{
    public class PlayerIdleState : ActionablePlayerState
    {
        public PlayerIdleState(IMachineOwner owner, StateSO stateData) : base(owner, stateData)
        {
        }

        protected override bool OnUpdate()
        {
            Vector2 inputDirection = _player.PlayerInput.InputDirection;
            if (inputDirection.sqrMagnitude > MOVE_THRESHOLD)
            {
                _player.ChangeState(PlayerState.MOVE);
                return false;
            }
            return true;
        }
    }
}