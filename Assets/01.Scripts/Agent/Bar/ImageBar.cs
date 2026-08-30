using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts.Agent.Bar
{
    public class ImageBar : AbstractBar
    {
        [SerializeField] private List<Image> targetImages;
        public override void HandleValueChanged(float numerator, float demominator)
        {
            float value = numerator / demominator;
            foreach (Image targetImage in targetImages)
                targetImage.DOFillAmount(value, tweenStep.Duration).SetEase(tweenStep.EaseType);
        }
    }
}