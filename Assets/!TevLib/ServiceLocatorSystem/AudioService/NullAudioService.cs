using _TevLib.SoundSystem;
using _TevLib.SoundSystem.Runtime;
using UnityEngine;

namespace _TevLib.ServiceLocatorSystem.AudioService
{
    public class NullAudioService : IAudioService
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void SetDefaultService()
        => ServiceLocator.RegisterService<IAudioService>(new NullAudioService());
        public void PlaySfx(SoundClipSO clipData, int channel = 0) { }

        public void StopSfx(int channel = 0) { }

        public void PlayBgm(SoundClipSO bgmSound) { }

        public void StopBgm() { }
    }
}