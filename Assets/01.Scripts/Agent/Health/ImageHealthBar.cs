using _TevLib.Extension.DoT;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts.Agent.Health
{
    public class ImageHealthBar : AbstractHealthBar
    {
        [SerializeField] private Image healthBar;
        public override void HandleHealthChanged(float health, float maxHealth)
        {
            float healthPer = health / maxHealth;
            healthBar.DOFillAmount(healthPer, tweenStep.Duration).SetEase(tweenStep.EaseType);
        }
    }
}