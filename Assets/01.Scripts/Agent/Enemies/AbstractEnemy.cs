using _01.Scripts.Agent.Enemies.BT;
using _01.Scripts.Agent.Enemies.BT.Event;
using _01.Scripts.SkillSystem;
using _TevLib.TileAstar;
using Unity.Behavior;
using UnityEngine;

namespace _01.Scripts.Agent.Enemies
{
    public abstract class AbstractEnemy : AbstractAgent
    {
        [SerializeField] private bool isDebugMode;
        [field:SerializeField] public EnemyDataSO EnemyData { get; private set;}
        [SerializeField] private bool useHitState;
        public NavModule Nav {get; private set;}    
        public EnemySensor Sensor { get; private set; }
        public ISkillModule SkillModule { get; private set; }


        #region BT

        public BehaviorGraphAgent BtAgent { get; private set; }
        private StateChannel _stateChannel;

        #endregion
        

        protected override void InitializeModules()
        {
            base.InitializeModules();
            
            BtAgent = GetComponent<BehaviorGraphAgent>();
            
            Sensor = GetModule<EnemySensor>();
            Nav = GetModule<NavModule>();
            SkillModule = GetModule<ISkillModule>();
        }

        private void Start()
        {
            BtAgent.SetVariableValue(BtVars.Enemy, this);

            if (BtAgent.GetVariable(BtVars.StateChannel, out BlackboardVariable<StateChannel> stateChannel))
                _stateChannel = stateChannel;
            else
                Debug.Log($"StateChannel이 없습니다 : {gameObject}");
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
        }

        private void OnDrawGizmos()
        {
            if (!isDebugMode || EnemyData == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position , EnemyData.AttackRange);      
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, EnemyData.DetectRadius);     
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position , EnemyData.SignalLostRange);
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position , EnemyData.StopDistance);
        }
    }
}