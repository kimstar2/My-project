using _01.Scripts.GameSystem;
using _01.Scripts.SkillSystem;
using _TevLib.FsmSystem.Runtime;
using UnityEngine;

namespace _01.Scripts.Agent.Player.Fsm
{
    public class PlayerSkillState : AbstractPlayerState
    {
        private PlayerSkillModule _skillModule;
        private ISkill _currentSkill;
        private bool _isSkillEnd;
        private SkillSlot? _castInputSlot;
        
        public PlayerSkillState(IMachineOwner owner, StateSO stateData) : base(owner, stateData)
        {
            _skillModule = _player.GetModule<PlayerSkillModule>();
        }

        public override void Enter()
        {
            // 애니메이션은 스킬마다 달라서 base.Enter() 안함
            _isSkillEnd = false;
            _skillModule.OnSkillEnd += HandleSkillEnd;

            _castInputSlot = _skillModule.RequestedInputSlot;
            
            ApplyAnimFacing(_skillModule.RequestedSkillId);
            
            _skillModule.UseSkill(_skillModule.RequestedSkillId);
            _currentSkill = _skillModule.CurrentSkill;
            
            _player.PlayerInput.OnAttackKeyPress += HandleAttackDuringSkill;
            _player.PlayerInput.OnSkillPerformed += HandleSkillDuringSkill;
        }

        protected override bool OnUpdate()
        {
            _currentSkill?.OnUpdateSkill();
            if (_isSkillEnd)
            {
                _player.ChangeState(PlayerState.IDLE);
                return false;
            }

            if (_currentSkill != null && _currentSkill.SkillData.canMove)
            {
                Vector2 inputDirection = _player.PlayerInput.InputDirection;
            
                _player.Mover.SetDirection(inputDirection);
            }
                
            
            return true;
        }

        public override void Exit()
        {
            _player.PlayerInput.OnAttackKeyPress -= HandleAttackDuringSkill;
            _player.PlayerInput.OnSkillPerformed -= HandleSkillDuringSkill;
            
            _skillModule.OnSkillEnd -= HandleSkillEnd;
            
            if (_currentSkill is {IsUsing:true})
                _currentSkill.StopSkill();
            base.Exit();
        }

        private void HandleSkillEnd(int skillId) => _isSkillEnd = true;

        private void ApplyAnimFacing(int skillId)
        {
            SkillDataSO skillData = _skillModule.GetSkillData(skillId); 
            if (skillData == null || skillData.directionType != DirectionType.Pointer) return;

            Vector3 mouseWorldPosition = _player.PlayerInput.GetPointerToWorldPos();
            Vector2 direction = ((Vector2)(mouseWorldPosition - _player.transform.position)).normalized;
            _player.Renderer.SetMovementDirection(direction);
        }
        
        #region 시전중 입력 처리 Handler

        private void HandleAttackDuringSkill(bool isPressed)
        {
            if (!isPressed) // 키가 떼진거니까 차징 종료
            {
                if (_castInputSlot == null)
                    _currentSkill?.OnReleaseInput();
                return;
            }

            if (_skillModule.TryResolveBasicAttack(out int id))
                TryCancelInto(id, null);
        }

        private void HandleSkillDuringSkill(SkillSlot slot, bool isPressed)
        {
            if (!isPressed)
            {
                if (_castInputSlot == slot)
                    _currentSkill?.OnReleaseInput();
                return;
            }
            if (_skillModule.TryResolveSlot(slot , out int id))
                TryCancelInto(id , slot);
        }

        private void TryCancelInto(int skillId , SkillSlot? inputSlot)
        {
            if (_currentSkill is { CanInterrupt: true } && _skillModule.TryRequestSkill(skillId , inputSlot))
                _player.ChangeState(PlayerState.SKILL);
        }

        #endregion
    }
}