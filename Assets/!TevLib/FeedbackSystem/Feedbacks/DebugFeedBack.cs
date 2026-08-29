using UnityEngine;

namespace _TevLib.FeedbackSystem.Feedbacks
{
    public class DebugFeedBack : AbstractFeedback
    {
        [SerializeField] private bool onDebug;
        [SerializeField] private string debug;
        public override void PlayFeedback()
        {
            if (onDebug)
                Debug.Log($"{debug} : {gameObject}");
        }

        public override void StopFeedback()
        {
            if (onDebug)
                Debug.Log($"Debug stopped {gameObject}");
        }
    }
}