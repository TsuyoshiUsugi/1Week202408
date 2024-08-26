using System.Collections.Generic;
using UnityEngine;

namespace UnityRoomProject.Audio
{
    [CreateAssetMenu(fileName = "AudioClipRegistrar", menuName = "Audio/AudioClipRegistrar")]
    public class AudioClipRegistrar : ScriptableObject
    {
        [SerializeField] private List<AudioClip> bgmClips;
        [SerializeField] private List<AudioClip> seClips;
        [SerializeField] private List<AudioClip> voiceClips;

        public List<AudioClip> BGMClips => bgmClips;
        public List<AudioClip> SEClips => seClips;
        public List<AudioClip> VoiceClips => voiceClips;
    }
}