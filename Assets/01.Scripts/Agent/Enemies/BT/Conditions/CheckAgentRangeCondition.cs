using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace _01.Scripts.Agent.Enemies.BT.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(name: "Check Agent Range", story: "[Enemy] to [TargetGO] [Operator] [Range]", category: "Conditions", id: "23191cbc48fe54b43ad8edb5a913e428")]
    public partial class CheckAgentRangeCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> TargetGO;
        [Comparison(comparisonType: ComparisonType.All)]
        [SerializeReference] public BlackboardVariable<ConditionOperator> Operator;
        [SerializeReference] public BlackboardVariable<RangeField> Range;

        public override bool IsTrue()
        {
            if (Enemy.Value == null || TargetGO.Value == null || Enemy.Value.EnemyData == null) return false;

            float threshold = Enemy.Value.EnemyData.GetFieldValue(Range.Value);
            float distance = Vector2.Distance(Enemy.Value.transform.position, TargetGO.Value.transform.position);

            return Operator.Value switch
            {
                ConditionOperator.Equal => Mathf.Approximately(distance, threshold),
                ConditionOperator.NotEqual => !Mathf.Approximately(distance, threshold),
                ConditionOperator.Greater => distance > threshold,
                ConditionOperator.Lower => distance < threshold,
                ConditionOperator.GreaterOrEqual => distance >= threshold,
                ConditionOperator.LowerOrEqual => distance <= threshold,
                _ => false
            };
        }
    }
}

