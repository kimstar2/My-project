using _01.Scripts.GameSystem;
using _TevLib.FsmSystem.Runtime;
using UnityEngine;

namespace _01.Scripts.Agent.Player.Fsm
{
    public abstract class ActionablePlayerState : AbstractPlayerState
    {
        private PlayerSkillModule _skillModule;
        protected ActionablePlayerState(IMachineOwner owner, StateSO stateData) : base(owner, stateData)
        {
            _skillModule = _player.GetModule<PlayerSkillModule>();
            Debug.Assert(_skillModule != null, "SkillModule is null");
        }

        public override void Enter()
        {
            base.Enter();
            _player.PlayerInput.OnAttackKeyPress += HandleAttackKey;
            _player.PlayerInput.OnSkillPerformed += HandleSkillKey;
        }

        public override void Exit()
        {
            _player.PlayerInput.OnAttackKeyPress -= HandleAttackKey;
            _player.PlayerInput.OnSkillPerformed -= HandleSkillKey;
            base.Exit();
        }

        private void HandleAttackKey(bool isPressed)
        {
            if (isPressed && _skillModule.TryResolveBasicAttack(out int id))
                TryEnterSkill(id);
        }

        private void HandleSkillKey(SkillSlot slot, bool isPressed)
        {
            if (isPressed && _skillModule.TryResolveSlot(slot , out int id))
                TryEnterSkill(id,slot);
        }
        
        private void TryEnterSkill(int skillId, SkillSlot? inputSlot = null)
        {
            if (_skillModule.TryRequestSkill(skillId, inputSlot))
                _player.ChangeState(PlayerState.SKILL);
        }
    }
}