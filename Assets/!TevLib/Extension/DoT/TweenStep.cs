using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace _TevLib.Extension.DoT
{
    [Serializable]
    public struct TweenStep
    {
        [field: SerializeField] public SequenceActionType ActionType { get; private set; }
        [field: SerializeField] public SequenceInsertType InsertType { get; private set; }
        [field: SerializeField] public Ease EaseType { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        
        [field:SerializeField] public Vector3 TransformValue { get; private set; }
        [field:SerializeField] public float FadeValue { get; private set; }
        [field:SerializeField] public Color ColorValue { get; private set; }
        
        [field: SerializeField] public UnityEvent Callback { get; private set; }

    }
}
