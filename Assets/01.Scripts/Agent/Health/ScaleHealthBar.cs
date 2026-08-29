using _TevLib.Extension.DoT;
using DG.Tweening;
using UnityEngine;

namespace _01.Scripts.Agent.Health
{
    public class ScaleHealthBar : AbstractHealthBar
    {
        public override void HandleHealthChanged(float health, float maxHealth)
        {
            float healthPer = health / maxHealth;
            transform.DOScaleX(healthPer, tweenStep.Duration).SetEase(tweenStep.EaseType);

        }
    }
}