using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

#if UNITY_EDITOR
namespace _01.Scripts.Agent.Enemies.BT.Event
{
    [CreateAssetMenu(menuName = "Behavior/Event Channels/StateChannel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "StateChannel", message: "Change [State]", category: "Events", id: "a5e8a1aef5d14427701eed689ecde83b")]
    public sealed partial class StateChannel : EventChannel<EnemyState> { }
}

