using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityRoomProject.Audio
{
    public class AudioManager : System.Singleton<AudioManager>, System.IPause
    {
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _seSource;
        [SerializeField] private AudioSource _voiceSource;

        [SerializeField] private AudioClipRegistrar _audioClipRegistrar;

        private Dictionary<BGMType, AudioClip> _bgmDictionary;
        private Dictionary<SEType, AudioClip> _seDictionary;
        private Dictionary<VoiceType, AudioClip> _voiceDictionary;

        private List<AudioSource> _activeSESources;
        private List<AudioSource> _activeVoiceSources;

        protected override bool UseDontDestroyOnLoad => true;

        private float _masterVolume = 1f;
        private float _bgmVolume = 1f;
        private float _seVolume = 1f;
        private float _voiceVolume = 1f;
        private const float Diff = 0.01f;

        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                if (!(_masterVolume + Diff < value) && !(_masterVolume - Diff > value)) return;
                _masterVolume = value;
                UpdateVolumes();
            }
        }

        public float BGMVolume
        {
            get => _bgmVolume;
            set
            {
                if (!(_bgmVolume + Diff < value) && !(_bgmVolume - Diff > value)) return;
                _bgmVolume = value;
                _bgmSource.volume = _bgmVolume * _masterVolume;
            }
        }

        public float SEVolume
        {
            get => _seVolume;
            set
            {
                if (!(_seVolume + Diff < value) && !(_seVolume - Diff > value)) return;
                _seVolume = value;
                UpdateActiveSEVolumes();
            }
        }

        public float VoiceVolume
        {
            get => _voiceVolume;
            set
            {
                if (!(_voiceVolume + Diff < value) && !(_voiceVolume - Diff > value)) return;
                _voiceVolume = value;
                UpdateActiveVoiceVolumes();
            }
        }

        private void Awake()
        {
            _bgmDictionary = new Dictionary<BGMType, AudioClip>();
            _seDictionary = new Dictionary<SEType, AudioClip>();
            _voiceDictionary = new Dictionary<VoiceType, AudioClip>();

            for (int i = 0; i < _audioClipRegistrar.BGMClips.Count; i++)
            {
                if (i < Enum.GetValues(typeof(BGMType)).Length)
                {
                    _bgmDictionary[(BGMType)i] = _audioClipRegistrar.BGMClips[i];
                }
            }

            for (int i = 0; i < _audioClipRegistrar.SEClips.Count; i++)
            {
                if (i < Enum.GetValues(typeof(SEType)).Length)
                {
                    _seDictionary[(SEType)i] = _audioClipRegistrar.SEClips[i];
                }
            }

            for (int i = 0; i < _audioClipRegistrar.VoiceClips.Count; i++)
            {
                if (i < Enum.GetValues(typeof(VoiceType)).Length)
                {
                    _voiceDictionary[(VoiceType)i] = _audioClipRegistrar.VoiceClips[i];
                }
            }

            _activeSESources = new List<AudioSource>();
            _activeVoiceSources = new List<AudioSource>();
        }

        private void UpdateVolumes()
        {
            _bgmSource.volume = _bgmVolume * _masterVolume;
            UpdateActiveSEVolumes();
            UpdateActiveVoiceVolumes();
        }

        private void UpdateActiveSEVolumes()
        {
            foreach (var source in _activeSESources)
            {
                source.volume = _seVolume * _masterVolume;
            }
        }

        private void UpdateActiveVoiceVolumes()
        {
            foreach (var source in _activeVoiceSources)
            {
                source.volume = _voiceVolume * _masterVolume;
            }
        }

        public void PlayBGM(BGMType type, bool loop = true)
        {
            if (_bgmDictionary.TryGetValue(type, out var clip))
            {
                _bgmSource.clip = clip;
                _bgmSource.loop = loop;
                _bgmSource.Play();
            }
            else
            {
                Debug.LogWarning($"BGM clip '{type}' not found. Cannot play BGM.");
            }
        }

        public async UniTask PlayBGMWithFade(BGMType type, float fadeDuration = 1f)
        {
            if (_bgmDictionary.TryGetValue(type, out var newClip))
            {
                if (_bgmSource.isPlaying)
                {
                    // フェードアウト
                    await FadeOutBGM(fadeDuration);
                }

                // フェードインの準備：音量を0に設定
                _bgmSource.volume = 0;

                // 新しいBGMの再生
                _bgmSource.clip = newClip;
                _bgmSource.Play();

                // フェードイン
                await FadeInBGM(fadeDuration);
            }
            else
            {
                Debug.LogWarning($"BGM clip '{type}' not found. Cannot play BGM.");
            }
        }

        private async UniTask FadeOutBGM(float duration)
        {
            float startVolume = _bgmSource.volume;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _bgmSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
                await UniTask.Yield(); // フレームごとに待機
            }

            _bgmSource.volume = 0;
            _bgmSource.Stop();
        }

        private async UniTask FadeInBGM(float duration)
        {
            _bgmSource.volume = 0;
            float targetVolume = _bgmVolume * _masterVolume;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _bgmSource.volume = Mathf.Lerp(0, targetVolume, t / duration);
                await UniTask.Yield(); // フレームごとに待機
            }

            _bgmSource.volume = targetVolume;
        }

        public int PlaySE(SEType type)
        {
            if (_seDictionary.TryGetValue(type, out var clip))
            {
                var source = CreateAudioSource(_seSource, "SE_" + type.ToString());
                source.PlayOneShot(clip, _seVolume * _masterVolume);
                _activeSESources.Add(source);
                return _activeSESources.Count - 1; // 追加したSEのインデックスを返す
            }
            else
            {
                Debug.LogWarning($"SE clip '{type}' not found. Cannot play SE.");
                return -1; // エラーを示すために-1を返す
            }
        }

        public int PlayVoice(VoiceType type)
        {
            if (_voiceDictionary.TryGetValue(type, out var clip))
            {
                var source = CreateAudioSource(_voiceSource, "Voice_" + type.ToString());
                source.PlayOneShot(clip, _voiceVolume * _masterVolume);
                _activeVoiceSources.Add(source);
                return _activeVoiceSources.Count - 1; // 追加したVoiceのインデックスを返す
            }
            else
            {
                Debug.LogWarning($"Voice clip '{type}' not found. Cannot play Voice.");
                return -1; // エラーを示すために-1を返す
            }
        }

        public void StopBGM()
        {
            _bgmSource.Stop();
        }

        public void StopSE(int index)
        {
            if (index >= 0 && index < _activeSESources.Count)
            {
                var source = _activeSESources[index];
                source.Stop();
                _activeSESources.RemoveAt(index);
                Destroy(source.gameObject);
            }
            else
            {
                Debug.LogWarning($"Invalid SE index '{index}' to stop.");
            }
        }

        public void StopSE(SEType type)
        {
            for (int i = 0; i < _activeSESources.Count; i++)
            {
                if (_activeSESources[i].name.Contains(type.ToString()))
                {
                    _activeSESources[i].Stop();
                    Destroy(_activeSESources[i].gameObject);
                    _activeSESources.RemoveAt(i);
                    return;
                }
            }
        }


        public void StopAllSE()
        {
            foreach (var source in _activeSESources)
            {
                source.Stop();
                Destroy(source.gameObject);
            }

            _activeSESources.Clear();
        }

        public void StopVoice(int index)
        {
            if (index >= 0 && index < _activeVoiceSources.Count)
            {
                var source = _activeVoiceSources[index];
                source.Stop();
                _activeVoiceSources.RemoveAt(index);
                Destroy(source.gameObject);
            }
            else
            {
                Debug.LogWarning($"Invalid Voice index '{index}' to stop.");
            }
        }

        public void Stop(VoiceType type)
        {
            for (int i = 0; i < _activeVoiceSources.Count; i++)
            {
                if (_activeVoiceSources[i].name.Contains(type.ToString()))
                {
                    _activeVoiceSources[i].Stop();
                    Destroy(_activeVoiceSources[i].gameObject);
                    _activeVoiceSources.RemoveAt(i);
                    return;
                }
            }
        }

        public void StopAllVoice()
        {
            foreach (var source in _activeVoiceSources)
            {
                source.Stop();
                Destroy(source.gameObject);
            }

            _activeVoiceSources.Clear();
        }

        private AudioSource CreateAudioSource(AudioSource template, string name)
        {
            var source = Instantiate(template, transform);
            source.name = name;
            source.volume = template.volume;
            return source;
        }

        public void Pause()
        {
            _bgmSource.Pause();

            foreach (var source in _activeSESources)
            {
                source.Pause();
            }

            foreach (var source in _activeVoiceSources)
            {
                source.Pause();
            }
        }

        public void Resume()
        {
            _bgmSource.UnPause();

            foreach (var source in _activeSESources)
            {
                source.UnPause();
            }

            foreach (var source in _activeVoiceSources)
            {
                source.UnPause();
            }
        }
    }
}