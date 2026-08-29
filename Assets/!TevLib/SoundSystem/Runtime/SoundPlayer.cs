using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _TevLib.SoundSystem.Runtime
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        [SerializeField] private AudioMixerGroup bgmMixerGroup;

        private AudioSource _audioSource;

        public event Action<SoundPlayer> OnSoundFinished;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound(SoundClipSO clipData)
        {
            if (clipData.audioType == AudioType.Sfx)
                _audioSource.outputAudioMixerGroup = sfxMixerGroup;
            else if (clipData.audioType == AudioType.Bgm)
                _audioSource.outputAudioMixerGroup = bgmMixerGroup;
            
            _audioSource.volume = clipData.volume;
            _audioSource.pitch = clipData.pitch;
            
            if (clipData.randomizePitch)
                _audioSource.pitch += Random.Range(-clipData.randomPitchModifier, clipData.randomPitchModifier);
            
            _audioSource.clip = clipData.clip;
            _audioSource.loop = clipData.isLoop;
            
            float startTime = clipData.startTime;
            float endTime = clipData.endTime;

            _audioSource.timeSamples = Mathf.RoundToInt(startTime * clipData.clip.frequency); // frequency = 주파수
            _audioSource.Play();

            if (!clipData.isLoop)
            {
                float duration = (endTime - startTime) / Mathf.Abs(_audioSource.pitch);
                DisableSoundTimer(duration + 0.2f).Forget();
            }
        }

        private async UniTaskVoid DisableSoundTimer(float duration)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            _audioSource.Stop();
            OnSoundFinished?.Invoke(this);
        }

        public void ForceSoundStop()
        {
            _audioSource.Stop();
            OnSoundFinished?.Invoke(this);
        }
    }
}