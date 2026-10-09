using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _01.Scripts.Agent.Interface;
using _01.Scripts.CombatSystem;
using _01.Scripts.SkillSystem;
using _01.Scripts.Util;
using _TevLib.Extension.DoT;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoreMountains.Tools;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Scripts.Agent.Enemies.Skill
{
    public class EnemyDashSkill : AbstractSkill
    {
        [Header("Dash Settings")]
        [HorizontalLineAttribute(color: EColor.Gray)]
        [SerializeField] private bool useCurve;
        [HideIf("useCurve")]
        [SerializeField] private Ease knockbackEase; 
        
        [ShowIf("useCurve")]
        [SerializeField] private AnimationCurve knockbackCurve; 
        
        [SerializeField] private float dashRange = 5f;
        [SerializeField] private float dashTime = 2f;
        
     
        [Header("AttackRangeView Settings")]
        [HorizontalLineAttribute(color: EColor.Gray)]
        [SerializeField] private Transform pivotTransform;
        [SerializeField] private MonoSprite viewSprite;
        [SerializeField] private TweenStep onRangeStep;
        [SerializeField] private Vector3 onViewRange;     
        
        [SerializeField] private TweenStep offRangeStep;
        [SerializeField] private Vector3 offViewRange;
        
        [Header("Fade Settings")]
        [HorizontalLineAttribute(color: EColor.Gray)]
        [SerializeField] private TweenStep fadeInStep;
        [SerializeField] private TweenStep fadeOutStep;
        [SerializeField] private float fadeInValue,fadeOutValue;
        
        [Header("DamageCast Event")]
        [HorizontalLineAttribute(color: EColor.Gray)]
        public UnityEvent onStartDamageCast;
        public UnityEvent onEndDamageCast;
        
        // 컴포넌트 캐시
        private IRenderable _renderer;
        private IAnimatorTrigger _trigger;
        private IMoveable _mover;
        private AbstractTickDamageCaster _damageCaster;
        
        // 스킬 로직 쪽 유니태스크
        private CancellationTokenSource _logicCts;
        
        #region Override
        
        public override void InitializeSkill(ISkillModule skillModule)
        {
            base.InitializeSkill(skillModule);
            _renderer = skillModule.Owner.GetModule<IRenderable>();
            _trigger = skillModule.Owner.GetModule<IAnimatorTrigger>();
            _mover = skillModule.Owner.GetModule<IMoveable>();
            _damageCaster = GetComponentInChildren<AbstractTickDamageCaster>();
            _damageCaster?.InitCaster(skillModule.Owner);
        }

        public override bool CanUseSkill(GameObject target = null)
        {
            if (target == null) return false;
            if (IsUsing || NormalizedCooldown > 0f ) return false; // 사용중이거나 쿨타임이면
            
            return Vector2.Distance(transform.position, target.transform.position) <= SkillData.maxRange;
        }

        public override void UseSkill(GameObject target = null)
        {
            base.UseSkill(target);
            SkillAsync(target).Forget();
        }

        public override void CleanUpSkillData()
        {
            UTaskUtil.Kill(ref _logicCts);
            OffViewAttackRange();
            
            _trigger.OnAnimationEnd -= HandleSkillAnimationEnd;
            _trigger.OnStartDamageCast -= HandleStartDamageCast;
            _trigger.OnEndDamageCast -= HandleEndDamageCast;
            base.CleanUpSkillData();
        }
        
        #endregion

        #region Logic

        private async UniTask SkillAsync(GameObject target)
        {
            _mover.Stop();
            
            if (target != null && SkillData.directionType == DirectionType.Body)
            {
                Vector2 direction = target.transform.position - transform.position;
                Vector2 normalizeDir = direction.normalized;
                _renderer.SetMovementDirection(normalizeDir);

                UTaskUtil.Kill(ref _logicCts);
                _logicCts = new CancellationTokenSource();
                
                await UTaskUtil.TryUniTask(OnViewAttackRange(_logicCts.Token));
                OffViewAttackRange();
                Dash(normalizeDir);
            }

            _trigger.OnAnimationEnd -= HandleSkillAnimationEnd;
            _trigger.OnAnimationEnd += HandleSkillAnimationEnd;

            _trigger.OnStartDamageCast -= HandleStartDamageCast;
            _trigger.OnStartDamageCast += HandleStartDamageCast;

            _trigger.OnEndDamageCast -= HandleEndDamageCast;
            _trigger.OnEndDamageCast += HandleEndDamageCast;
            
            
            if (SkillData.defaultAnimHash != null)
                _renderer.RenderClip(SkillData.defaultAnimHash.HashValue);
        }
        
        private async UniTask OnViewAttackRange(CancellationToken ct)
        {
            TweenDelegator.SetDOFade(viewSprite.Sr,fadeInValue,fadeInStep);
            await pivotTransform.transform
                    .DOScale(onViewRange, onRangeStep.Duration)
                    .SetEase(onRangeStep.EaseType)
                    .ToUniTask(cancellationToken: ct);
        }
        
        private void OffViewAttackRange()
        {
            TweenDelegator.SetDOFade(viewSprite.Sr,fadeOutValue,fadeOutStep);
            pivotTransform.transform.DOScale(offViewRange, offRangeStep.Duration);
        }

        private void Dash(Vector2 normalizeDir)
        {
            if (useCurve)
                _mover.AddForce(normalizeDir * dashRange, dashTime,knockbackCurve ,ForceMode2D.Impulse);
            else
                _mover.AddForce(normalizeDir * dashRange, dashTime,knockbackEase,ForceMode2D.Impulse);
        }

        #endregion

        #region Handle
        
        private void HandleStartDamageCast()
        {
            Vector2 direction = _renderer.FacingDirection;
            float damage = SkillModule.GetBaseDamage(SkillData);
            
            _damageCaster.OnCastDamage(damage,direction,SkillData.kbForce);
            
            onStartDamageCast?.Invoke();
        }
        
        private void HandleEndDamageCast()
        {
            Debug.Log("EndDamageCast");
            _damageCaster.OffCastDamage();
            onEndDamageCast?.Invoke();
        }
        private void HandleSkillAnimationEnd()
        {
            StopSkill();
        }

        #endregion
    }
}