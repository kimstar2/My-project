using _TevLib.Extension.DoT;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace _01.Scripts.Util
{
    public static class TweenDelegator
    {

        public static void SetDOFade(SpriteRenderer r, float fadeVal , TweenStep step)
        {
            var t = r.DOFade(fadeVal,step.Duration);
            AddPropFromStep(t, step);
        }
        public static void AddPropFromStep(Tweener t,TweenStep step)
        {
            t.SetEase(step.EaseType);
        }   
        public static void AddPropFromStep(Tween t ,TweenStep step , GameObject link)
        {
            t.SetEase(step.EaseType).SetLink(link);
        }  
    }
}