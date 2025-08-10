using System.Collections;
using Code.Audio.General;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Code.Audio
{
    public static class AudioManager
    {
        private const string KMasterStatusKey = "CM.MASTER_STATUS_KEY_PREFERENCE_CODE";
        private const string KMasterVolumeKey = "CM.MASTER_VOLUME_KEY_PREFERENCE_CODE";

        private const string KMusicStatusKey = "CM.MUSIC_STATUS_KEY_PREFERENCE_CODE";
        private const string KMusicVolumeKey = "CM.MUSIC_VOLUME_KEY_PREFERENCE_CODE";

        private const string KSfxVolumeKey = "CM.SFX_VOLUME_KEY_PREFERENCE_CODE";
        private const string KSfxStatusKey = "CM.SFX_STATUS_KEY_PREFERENCE_CODE";

        private static AudioEvent[] Sounds;
        private static AudioMixer AudioMixer;
        private static AudioMixerGroup MasterGroup;
        private static AudioMixerGroup SfxGroup;
        private static AudioMixerGroup MusicGroup;

        private static AudioSource MusicSource; // Persistent music source
        private static bool PreferencesApplied = false;
        private const int MaxSfxAudioSources = 20; // for each Obj!

        static AudioManager()
        {
            InitAudioMixers();
            CacheSounds();
            CreatePersistentMusicObject();
            SetupAudioPreferences();
        }

        private static void InitAudioMixers()
        {
            AudioMixer = Resources.Load<AudioMixer>("AudioMixer");
            MasterGroup = AudioMixer.FindMatchingGroups("Master")[0];
            SfxGroup = AudioMixer.FindMatchingGroups("SFX")[0];
            MusicGroup = AudioMixer.FindMatchingGroups("Music")[0];
        }

        private static void CacheSounds()
        {
            Sounds = Resources.LoadAll<AudioEvent>("Audio/");
        }

        private static void CreatePersistentMusicObject()
        {
            GameObject musicObject = new("MusicManager");
            Object.DontDestroyOnLoad(musicObject);
            MusicSource = musicObject.AddComponent<AudioSource>();
            MusicSource.outputAudioMixerGroup = MusicGroup;
        }

        private static void SetupAudioPreferences()
        {
            ApplyAudioPreferences(AudioType.Master);
            ApplyAudioPreferences(AudioType.Music);
            ApplyAudioPreferences(AudioType.Sfx);
        }

        private static void ApplyAudioPreferences(AudioType type)
        {
            bool isAudioOn = GetSoundStatus(type);
            float volume = GetSoundVolume(type);

            SetAudioStatus(isAudioOn, type);
            SetVolume(volume, type);
        }

        public static void SetVolumeNoSave(float volume, AudioType type)
        {
            volume = Mathf.Clamp(volume, 0, 100);

            AudioMixerGroup mixerGroup = type switch
            {
                AudioType.Master => MasterGroup,
                AudioType.Music => MusicGroup,
                AudioType.Sfx => SfxGroup,
                _ => throw new System.ArgumentException($"Invalid AudioType: {type}")
            };

            var parameterName = type.ToString().ToLower() + "Vol";
            float dB = ConvertVolumeToDB(volume);

            mixerGroup.audioMixer.SetFloat(parameterName, dB);
        }
        
        public static void SetVolume(float volume, AudioType type)
        {
            volume = Mathf.Clamp(volume, 0, 100);

            AudioMixerGroup mixerGroup = type switch
            {
                AudioType.Master => MasterGroup,
                AudioType.Music => MusicGroup,
                AudioType.Sfx => SfxGroup,
                _ => throw new System.ArgumentException($"Invalid AudioType: {type}")
            };

            var parameterName = type.ToString().ToLower() + "Vol";
            float dB = ConvertVolumeToDB(volume);

            mixerGroup.audioMixer.SetFloat(parameterName, dB);
            PlayerPrefs.SetFloat(type switch
            {
                AudioType.Master => KMasterVolumeKey,
                AudioType.Music => KMusicVolumeKey,
                AudioType.Sfx => KSfxVolumeKey,
                _ => throw new System.ArgumentException($"Invalid AudioType: {type}")
            }, volume);
        }

        public static void SetAudioStatus(bool isAudioOn, AudioType type)
        {
            AudioMixerGroup mixerGroup = type switch
            {
                AudioType.Master => MasterGroup,
                AudioType.Music => MusicGroup,
                AudioType.Sfx => SfxGroup,
                _ => throw new System.ArgumentException($"Invalid AudioType: {type}")
            };

            string parameterName = type.ToString().ToLower() + "Vol";
            float volume = isAudioOn ? GetSoundVolume(type) : 0;

            PlayerPrefs.SetInt(type switch
            {
                AudioType.Master => KMasterStatusKey,
                AudioType.Music => KMusicStatusKey,
                AudioType.Sfx => KSfxStatusKey,
                _ => throw new System.ArgumentException($"Invalid AudioType: {type}")
            }, isAudioOn ? 1 : 0);

            mixerGroup.audioMixer.SetFloat(parameterName, isAudioOn
                ? ConvertVolumeToDB(volume)
                : -80f);
        }

        public static bool GetSoundStatus(AudioType type)
        {
            return type switch
            {
                AudioType.Master => PlayerPrefs.GetInt(KMasterStatusKey, 1) == 1,
                AudioType.Music => PlayerPrefs.GetInt(KMusicStatusKey, 1) == 1,
                AudioType.Sfx => PlayerPrefs.GetInt(KSfxStatusKey, 1) == 1,
                _ => throw new System.ArgumentException($"Invalid AudioType: {type}")
            };
        }

        public static float GetSoundVolume(AudioType type)
        {
            return type switch
            {
                AudioType.Master => PlayerPrefs.GetFloat(KMasterVolumeKey, 100.0f),
                AudioType.Music => PlayerPrefs.GetFloat(KMusicVolumeKey, 100.0f),
                AudioType.Sfx => PlayerPrefs.GetFloat(KSfxVolumeKey, 100.0f),
                _ => throw new System.ArgumentException($"Invalid AudioType: {type}")
            };
        }

        private static float ConvertVolumeToDB(float volume)
        {
            if (volume <= 0) return -80f;
            return Mathf.Log10(volume / 100f) * 20f;
        }

        public static void PlayAudio(AudioID audioID, MonoBehaviour caller, float fadeDuration = 1f)
        {
            if (!PreferencesApplied)
            {
                SetupAudioPreferences();
                PreferencesApplied = true;
            }

            foreach (var sound in Sounds)
            {
                if (sound.name == audioID.ToString())
                {
                    if (sound is AudioEventMusic musicEvent)
                    {
                        PlayMusic(musicEvent, fadeDuration);
                    }
                    else if (sound is AudioEventSfx sfxEvent)
                    {
                        DOPlaySfx(sfxEvent, caller.gameObject);
                    }
                    return;
                }
            }
        }
        
        public static void PlayMusic(AudioEventMusic musicEvent, float fadeDuration)
        {
            if (musicEvent == null) return;

            PlayAudioEvent(musicEvent, MusicSource);
            EnsureMonoBehaviour(MusicSource.gameObject).StartCoroutine(FadeInMusic(fadeDuration));
        }

        private static void DOPlaySfx(AudioEventSfx sfxEvent, GameObject caller)
        {
            if (sfxEvent == null || caller == null) return;

            AudioSource sfxSource = GetOrCreateAudioSource(caller);
            if (sfxSource == null) return;

            sfxSource.outputAudioMixerGroup = SfxGroup;
            sfxSource.loop = sfxEvent.Loop;
            PlayAudioEvent(sfxEvent, sfxSource);
        }

        private static AudioSource GetOrCreateAudioSource(GameObject caller)
        {
            AudioSource[] sources = caller.GetComponents<AudioSource>();
            foreach (AudioSource source in sources)
            {
                if (!source.isPlaying)
                    return source;
            }

            return sources.Length < MaxSfxAudioSources ? caller.AddComponent<AudioSource>() : sources[0];
        }

        public static void StopMusic(float fadeDuration = 1f)
        {
            if (MusicSource == null || !MusicSource.isPlaying) return;

            EnsureMonoBehaviour(MusicSource.gameObject).StartCoroutine(FadeOutMusic(fadeDuration));
        }

        private static IEnumerator FadeInMusic(float duration)
        {
            float targetVolume = PlayerPrefs.GetFloat(KMusicVolumeKey, 100.0f) / 100f;
            float elapsed = 0f;

            MusicSource.volume = 0f;
            MusicSource.Play();

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                MusicSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
                yield return null;
            }

            MusicSource.volume = targetVolume;
        }

        private static IEnumerator FadeOutMusic(float duration)
        {
            float startVolume = MusicSource.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                MusicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }

            MusicSource.volume = 0f;
            MusicSource.Stop();
        }

        private static MonoBehaviour EnsureMonoBehaviour(GameObject caller)
        {
            var mono = caller.GetComponent<MonoBehaviour>();
            if (mono == null)
            {
                mono = caller.AddComponent<TemporaryMonoBehaviour>();
            }
            return mono;
        }

        private class TemporaryMonoBehaviour : MonoBehaviour { }

        public static void StopAudio(AudioID audioID, GameObject caller)
        {
            foreach (var sound in Sounds)
            {
                if (sound.name == audioID.ToString())
                {
                    var sources = caller.GetComponents<AudioSource>();
                    foreach (var source in sources)
                    {
                        source.Stop();
                        Object.Destroy(source);
                    }
                    return;
                }
            }
        }

        private static void PlayAudioEvent(AudioEvent audioEvent, AudioSource audioSource)
        {
            if (audioEvent is AudioEventSfx sfxEvent)
            {
                if (sfxEvent.Use3DSound)
                {
                    audioSource.spatialBlend = 1.0f; // 3D sound
                    if (sfxEvent.UseAdvancedOptions)
                    {
                        audioSource.dopplerLevel = sfxEvent.GetDopplerLevel();
                        audioSource.spread = sfxEvent.GetSpread();
                        audioSource.rolloffMode = sfxEvent.GetVolumeRolloff();
                        audioSource.minDistance = sfxEvent.GetMinDistance();
                    }
                }
                else
                {
                    audioSource.spatialBlend = 0.0f; // 2D sound
                    if (sfxEvent.UseAdvancedOptions)
                    {
                        audioSource.panStereo = sfxEvent.GetStereoPan();
                        audioSource.priority = sfxEvent.GetPriority();
                    }
                }
            }

            audioSource.clip = audioEvent.Clips[Random.Range(0, audioEvent.Clips.Length)];
            audioSource.loop = audioEvent.Loop;
            audioSource.volume = audioEvent.Volume.maxValue;
            
            audioSource.Play();
        }
    }
}