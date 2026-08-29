using System;
using UnityEngine;

namespace _TevLib.Extension.DoT
{
    [Serializable]
    public enum SequenceActionType
    {
        [InspectorName("SingleTween")]
        DoTween,
        [InspectorName("DOAnchoredPosition")]
        DoAnchoredPosition,
        [InspectorName("DOCanvasAlpha")]
        DoCanvasAlpha,
        [InspectorName("DOLocalScale")]
        DoLocalScale, 
        [InspectorName("DOMove")]
        DoMove,
        [InspectorName("DOLocalRotation")]
        DoLocalRotation
    }
}