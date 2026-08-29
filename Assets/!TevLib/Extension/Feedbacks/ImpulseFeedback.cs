using System;
using System.Collections.Generic;
using _TevLib.FeedbackSystem;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _TevLib.Extension.Feedbacks
{
    [Serializable]
    public struct Vec3Range
    {
        public Vector3 min;
        public Vector3 max;
    }
    public class ImpulseFeedback : AbstractFeedback
    {
        [SerializeField] private CinemachineImpulseSource impulseSrc;
        [SerializeField] private List<Vec3Range> velocityRanges;
        [SerializeField] private float multiForce = 1f;

        public override void PlayFeedback()
        {
            Vector3 overrideVelocity = new Vector3(GetX(), GetY(), 0);
            impulseSrc.GenerateImpulseWithVelocity(overrideVelocity * multiForce);
        }

        private float GetX()
        {
            int r = Random.Range(0, velocityRanges.Count);
            float x = Random.Range(velocityRanges[r].min.x, velocityRanges[r].max.x);
            return x;
        }
        
        private float GetY()
        {
            int r = Random.Range(0, velocityRanges.Count);
            float y = Random.Range(velocityRanges[r].min.y, velocityRanges[r].max.y);
            return y;
        }

        public override void StopFeedback() => impulseSrc.StopAllCoroutines();
    }
}