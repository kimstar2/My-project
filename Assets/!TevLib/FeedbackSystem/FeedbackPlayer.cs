using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _TevLib.FeedbackSystem
{
    public class FeedbackPlayer : MonoBehaviour , IFeedbackPlayer
    {
        private List<AbstractFeedback> _feedbacks = new();

        private void Awake()
        {
            _feedbacks = GetComponentsInChildren<AbstractFeedback>(true).ToList();
        }

        public void AddFeedback(AbstractFeedback feedback)
        {
            if (feedback == null || _feedbacks.Contains(feedback))
            {
                Debug.LogWarning("[FeedbackPlayer] Feedback already exists");
                return;
            }
            _feedbacks.Add(feedback);
        }

        public void RemoveFeedback(AbstractFeedback feedback)
        {
            if (feedback == null || !_feedbacks.Contains(feedback))
            {
                Debug.LogWarning("[FeedbackPlayer] Feedback doesn't exists");
                return;
            }
            feedback.StopFeedback();
            _feedbacks.Remove(feedback);
        }

        public void PlayAllFeedback()
        {
            foreach (AbstractFeedback f in _feedbacks)
                f.PlayFeedback();
        }

        public void StopAllFeedback()
        {
            foreach (AbstractFeedback f in _feedbacks)
                f.StopFeedback();
        }
        

        public void ClearFeedback()
        {
            StopAllFeedback();
            _feedbacks.Clear();
        }

        public void SetFeedbackDir(Vector3 direction)
        {
            Vector2 dir = transform.position + direction - transform.position;
            
            foreach (AbstractFeedback f in _feedbacks)
            {
                if (f is IFeedbackDir fbA)
                    fbA.SetDirAndPlay(dir.normalized);
            }
        }
    }
}