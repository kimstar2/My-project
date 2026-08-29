    using System;
    using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace _01.Scripts.Agent.Enemies.BT.Action
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "ChaseToTargetAction", story: "[Enemy] chase to [TargetGO]", category: "Action", id: "5c95a9476c811db000c6067618c86f47")]
    public partial class ChaseToTargetAction : Unity.Behavior.Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> TargetGO;

        private AbstractEnemy _enemy;
        private NavModule _nav;
        
        protected override Status OnStart()
        {
            if (Enemy.Value == null || TargetGO.Value == null ) return Status.Failure;

            _enemy = Enemy.Value;
            _nav = _enemy.Nav;
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            GameObject target = TargetGO.Value;
            if (_enemy == null || target == null) return Status.Failure;
            
            Vector3 toTarget = target.transform.position - _enemy.transform.position;
            toTarget.z = 0;

            float stopDistance = _enemy.EnemyData.StopDistance;
            if (toTarget.sqrMagnitude <= stopDistance * stopDistance)
            {
                _nav.Stop();
                return Status.Success;
            }
            
            _nav.SetDestination(target.transform.position);
            return Status.Running;
        }

        protected override void OnEnd()
        {
            _nav?.Stop();
        }
    }
}

