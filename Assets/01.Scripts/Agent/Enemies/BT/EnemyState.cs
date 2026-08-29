using Unity.Behavior;

namespace _01.Scripts.Agent.Enemies.BT
{
    [BlackboardEnum]
    public enum EnemyState
    {
        IDLE,
        BATTLE,
        HIT,
        DEAD,
        STUN
    }
}