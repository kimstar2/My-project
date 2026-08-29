using _TevLib.FsmSystem.Runtime;
using UnityEngine;

namespace _01.Scripts.Agent.Player.Fsm
{
    public abstract class AbstractPlayerState : AbstractState
    {
        protected PlayerController _player;
        
        protected const float MOVE_THRESHOLD = 0.01f;
        
        public AbstractPlayerState(IMachineOwner owner, StateSO stateData) : base(owner, stateData)
        {
            _player = owner.GameObject.GetComponent<PlayerController>();
            Debug.Assert(_player != null,"PlayerController is null");
        }

        public override void Enter() => _player.Renderer.RenderClipIfNotPlaying(StateSo.animHash.HashValue);
    }
}