namespace _TevLib.FeedbackSystem
{
    public interface IFeedbackPlayer
    {
        void AddFeedback(AbstractFeedback feedback);
        void RemoveFeedback(AbstractFeedback feedback);
        void PlayAllFeedback();
        void StopAllFeedback();
        void ClearFeedback();
    }
}