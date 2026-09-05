using System;
using System.Threading;
using _01.Scripts.Agent.Enemies.BT;
using _01.Scripts.Agent.Enemies.BT.Event;
using _01.Scripts.CombatSystem;
using _01.Scripts.SkillSystem;
using _TevLib.ModuleSystem;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.PoolService;
using _TevLib.ServiceLocatorSystem.TimeService;
using _TevLib.TileAstar;
using Unity.Behavior;
using UnityEngine;

namespace _01.Scripts.Agent.Enemies
{
    public abstract class AbstractEnemy : AbstractAgent, IAfterInitModule, IPoolable
    {
        [SerializeField] private bool isDebugMode;

        [field: Header("EnemySetting"), SerializeField]
        public EnemyDataSO EnemyData { get; private set; }

        [SerializeField] private bool useHitState;
        [SerializeField] private float returnToPoolTime;

        private ITimeService _timeService;
        private IPoolingService _poolingService;
        private CancellationTokenSource _returnCts;

        #region ModuleCompo

        public NavModule Nav { get; private set; }
        public EnemySensor Sensor { get; private set; }
        public ISkillModule SkillModule { get; private set; }
        public HealthModule HealthModule { get; private set; }
        public HitBox HitBox { get; private set; }

        #endregion

        #region BT

        public BehaviorGraphAgent BtAgent { get; private set; }
        private StateChannel _stateChannel;

        #endregion


        protected override void InitializeModules()
        {
            base.InitializeModules();
            
            Sensor = GetModule<EnemySensor>();
            Nav = GetModule<NavModule>();
            SkillModule = GetModule<ISkillModule>();
            HealthModule = GetModule<HealthModule>();
            HitBox = GetModule<HitBox>();
            
            _timeService = ServiceLocator.GetService<ITimeService>();
            _poolingService = ServiceLocator.GetService<IPoolingService>();
        }

        public void AfterInit()
        {
            BtSet();

            HealthModule.onDead.AddListener(HandleDead);
            HealthModule.OnHit += HandleHit;
        }

        private void BtSet()
        {
            BtAgent = GetComponent<BehaviorGraphAgent>();
            
            BtAgent.SetVariableValue(BtVars.Enemy, this);
            
            if (BtAgent.GetVariable(BtVars.StateChannel, out BlackboardVariable<StateChannel> stateChannel))
                _stateChannel = stateChannel;
            else
                Debug.Log($"StateChannel이 없습니다 : {gameObject}");
        }

        private void OnDestroy()
        {
            HealthModule.onDead.RemoveListener(HandleDead);
            HealthModule.OnHit -= HandleHit;
        }

        protected override void HandleHit()
        {
            if (IsDead) return;
            if (useHitState)
                _stateChannel.SendEventMessage(EnemyState.HIT);
        }

        protected override void HandleDead()
        {
            base.HandleDead();
            _stateChannel.SendEventMessage(EnemyState.DEAD);
            WaitOnReturnToPool();
            HitBox.SetActive(false);
        }

        #region Pool

        [field: Header("Pool Setting"), SerializeField]
        public PoolItemSO Item { get; private set; }

        public GameObject GameObject => gameObject;

        public void ResetItem()
        {
            _stateChannel.SendEventMessage(EnemyState.IDLE);
            IsDead = false;
            HealthModule.HealthInit();
            HitBox.SetActive(true);
        }

        private void WaitOnReturnToPool()
        {
            KillTask();
            _returnCts = new();
            CancellationToken ct = _returnCts.Token;

            _timeService.ActionTimer(returnToPoolTime,
                ct,
                null,
                () => _poolingService.Push(this));
        }

        private void KillTask()
        {
            if (_returnCts != null)
            {
                _returnCts.Cancel();
                _returnCts.Dispose();
                _returnCts = null;
            }
        }

        #endregion


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!isDebugMode || EnemyData == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, EnemyData.AttackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, EnemyData.DetectRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, EnemyData.SignalLostRange);
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, EnemyData.StopDistance);
        }
#endif
    }
}