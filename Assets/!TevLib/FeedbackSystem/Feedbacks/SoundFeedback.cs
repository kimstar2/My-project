using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.AudioService;
using _TevLib.SoundSystem;
using _TevLib.SoundSystem.Runtime;
using UnityEngine;
using AudioType = _TevLib.SoundSystem.Runtime.AudioType;

namespace _TevLib.FeedbackSystem.Feedbacks
{
    public class SoundFeedback : AbstractFeedback
    {
        [SerializeField] private SoundClipSO soundClip;
        [SerializeField] private int channel = 0;
        
        public override void PlayFeedback()
        {
            if (soundClip == null)
            {
                Debug.LogWarning("[SoundFeedback] Sound clip is null");
                return;
            }
            
            if (soundClip.audioType == AudioType.Sfx)
                ServiceLocator.GetService<IAudioService>().PlaySfx(soundClip , channel);
            else if (soundClip.audioType == AudioType.Bgm)
                ServiceLocator.GetService<IAudioService>().PlayBgm(soundClip);
        }

        public override void StopFeedback()
        {
            if (soundClip.audioType == AudioType.Sfx)
                ServiceLocator.GetService<IAudioService>().StopSfx(channel);
            else if (soundClip.audioType == AudioType.Bgm)
                ServiceLocator.GetService<IAudioService>().StopBgm();
        }
    }
}