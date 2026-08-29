using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace _01.Scripts.Agent.Enemies.BT.Action
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Detect Target", story: "[Enemy] detect [TargetGO]", category: "Action/Combat", id: "934d0940467a3bf59d549ae269c3ba00")]
    public partial class DetectTargetAction : Unity.Behavior.Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> TargetGO;

        protected override Status OnStart()
        {
            AbstractEnemy enemy = Enemy.Value;
            if (enemy == null || enemy.Sensor == null || enemy.EnemyData == null)
                return Status.Failure;

            bool found = enemy.Sensor.TryDetectTarget(enemy.EnemyData.DetectRadius, out GameObject target);
            
            if (found)
            {
                TargetGO.Value = target;
                return Status.Success;
            }
            
            return Status.Failure;
        }
    }
}

