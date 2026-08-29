using _TevLib.SoundSystem;
using _TevLib.SoundSystem.Runtime;

namespace _TevLib.ServiceLocatorSystem.AudioService
{
    public interface IAudioService
    {
        void PlaySfx(SoundClipSO clipData , int channel = 0);
        void StopSfx(int channel = 0);
        
        void PlayBgm(SoundClipSO bgmSound);
        void StopBgm();
    }
}