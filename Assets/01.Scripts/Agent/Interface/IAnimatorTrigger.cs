using System;

namespace _01.Scripts.Agent.Interface
{
    public interface IAnimatorTrigger
    {
        event Action OnAnimationEnd;
        event Action OnDamageCast;
        event Action OnFootstep;
    }
}