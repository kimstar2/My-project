using UnityEngine;

namespace _TevLib.SoundSystem.Runtime
{
    public enum AudioType
    {
        Sfx,
        Bgm
    }
    [CreateAssetMenu(fileName = "Clip data", menuName = "TevLib/Sound/Clip data", order = 0)]
    public class SoundClipSO : ScriptableObject
    {
        public AudioType audioType;
        public AudioClip clip;
        public bool isLoop = false;
        public bool randomizePitch = false;
        
        [Range(0,1f)] public float randomPitchModifier = 0.1f;
        [Range(0,1f)] public float volume = 1f;
        [Range(0.1f,3f)] public float pitch = 1f;

        public float startTime = 0f;
        public float endTime = 0f;
    }
}