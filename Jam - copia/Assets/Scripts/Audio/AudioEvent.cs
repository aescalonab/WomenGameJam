using Code.Audio.General;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Code.Audio
{
    public enum AudioType
    {
        Master = 0,
        Music = 1,
        Sfx = 2
    }

    public abstract class AudioEvent : ScriptableObject
    {
        [FormerlySerializedAs("clips")] [Tooltip("AudioClip - Just Drag and Drop an audio or more files from your project here)")]
        public AudioClip[] Clips;

        [FormerlySerializedAs("volume")]
        [Tooltip("Volume range for this specific sound")]
        [MinMaxRange(0f, 1f)] public RangedFloat Volume = new RangedFloat { minValue = 1f, maxValue = 1f };

        [FormerlySerializedAs("pitch")]
        [Tooltip("Pitch range for this specific sound")]
        [MinMaxRange(0f, 2f)] public RangedFloat Pitch = new RangedFloat { minValue = 1f, maxValue = 1f };

        [FormerlySerializedAs("loop")] [Tooltip("Should this audio loop?")]
        public bool Loop;

        [Tooltip("Audio type, internal control")]
        protected AudioType Type;

        [FormerlySerializedAs("useAdvancedOptions")] [Tooltip("Enable advanced sound options")]
        public bool UseAdvancedOptions;

        // Summary:
        //     Pans a playing sound in a stereo way (left or right). This only applies to sounds
        //     that are Mono or Stereo.
        [FormerlySerializedAs("stereoPan")]
        [Tooltip("Pans a playing sound in a stereo way (left or right). This only applies to sounds that are Mono or Stereo.")]
        [SerializeField, ConditionalField("useAdvancedOptions")]
        [Range(-1f, 1f)]
        private float StereoPan;

        [FormerlySerializedAs("priority")]
        [Tooltip("Priority for 2D sound (0 = highest priority, 256 = lowest priority)")]
        [SerializeField, ConditionalField("useAdvancedOptions")]
        [Range(0, 256)]
        private int Priority = 128;

        protected AudioSource Source;

        public virtual void SetSource(AudioSource newSource, AudioMixerGroup mixer = null)
        {
            Source = newSource;

            // Assign a random clip
            Source.clip = Clips[Random.Range(0, Clips.Length)];

            // Set the mixer group if provided
            if (mixer != null)
            {
                Source.outputAudioMixerGroup = mixer;
            }

            // Apply volume, pitch, and looping
            Source.volume = Random.Range(Volume.minValue, Volume.maxValue);
            Source.pitch = Random.Range(Pitch.minValue, Pitch.maxValue);
            Source.loop = Loop; // Set looping behavior

            // Apply advanced options if enabled
            if (UseAdvancedOptions)
            {
                Source.panStereo = StereoPan;
                Source.priority = Priority;
            }
        }

        public void Play()
        {
            if (Clips.Length == 0) return;

            Source.clip = Clips[Random.Range(0, Clips.Length)];
            
            if (Type == AudioType.Sfx && !Loop)
            {
                Source.PlayOneShot(Source.clip);
            }
            else
            {
                Source.Play();
            }
        }

        public void Stop()
        {
            if (Source != null)
            {
                Source.Stop();
            }
            else
            {
                Debug.LogWarning($"Tried to stop audio, but AudioSource was null on {this.name}");
            }
        }

        public float GetStereoPan()
        {
            return StereoPan;
        }

        public int GetPriority()
        {
            return Priority;
        }
    }
}