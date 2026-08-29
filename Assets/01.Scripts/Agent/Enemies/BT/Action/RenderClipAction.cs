using System;
using _TevLib.HashDataSystem;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace _01.Scripts.Agent.Enemies.BT.Action
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Render Clip Action", story: "[Enemy] Render [Clip] [isRestart]", category: "Action/Animation", id: "c1f0dd20f465f4bde0db4a7c503218c5")]
    public partial class RenderClipAction : Unity.Behavior.Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<AnimHashSO> Clip;
        [SerializeReference] public BlackboardVariable<bool> IsRestart;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.Renderer == null || Clip.Value == null) 
                return Status.Failure;
        
            if (IsRestart.Value)
                Enemy.Value.Renderer.RenderClip(Clip.Value.HashValue);
            else
                Enemy.Value.Renderer.RenderClipIfNotPlaying(Clip.Value.HashValue);

            return Status.Success;
        }
    }
}

