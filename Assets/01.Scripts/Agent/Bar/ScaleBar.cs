using DG.Tweening;

namespace _01.Scripts.Agent.Bar
{
    public class ScaleBar : AbstractBar
    {
        public override void HandleValueChanged(float numerator, float demoninator)
        {
            float value = numerator / demoninator;
            transform.DOScaleX(value, tweenStep.Duration).SetEase(tweenStep.EaseType);
        }
    }
}