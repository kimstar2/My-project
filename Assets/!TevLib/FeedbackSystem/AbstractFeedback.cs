using UnityEngine;

namespace _TevLib.FeedbackSystem
{
    public abstract class AbstractFeedback : MonoBehaviour
    {
        public abstract void PlayFeedback();
        public abstract void StopFeedback();

        protected virtual void OnDisable() => StopFeedback();
    }
}