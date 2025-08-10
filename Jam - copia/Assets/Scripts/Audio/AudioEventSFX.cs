using System;
using Code.Audio.General;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Code.Audio
{
    [Serializable, CreateAssetMenu(menuName = "Audio Event/SFX")]
    public class AudioEventSfx : AudioEvent
    {
        [FormerlySerializedAs("use3DSound")] [Tooltip("Enable 3D sound settings (e.g., min/max distance)")]
        public bool Use3DSound;

        [FormerlySerializedAs("minDistance")] [ConditionalField("use3DSound"), Tooltip("Minimum distance for 3D sound attenuation (default: 1)")]
        public float MinDistance = 1f;

        [FormerlySerializedAs("maxDistance")]
        [ConditionalField("use3DSound"), Tooltip("Maximum distance for 3D sound attenuation (default: 500, max: 1,000,000)")]
        [Min(1.01f)] public float MaxDistance = 500f;

        [FormerlySerializedAs("dopplerLevel")]
        [Tooltip("Doppler level for 3D sound (0 = no effect, 5 = maximum effect)")]
        [SerializeField, ConditionalField("useAdvancedOptions", false, "use3DSound", false)]
        [Range(0f, 5f)]
        private float DopplerLevel = 1.0f;

        [FormerlySerializedAs("spread")]
        [Tooltip("Spread for 3D sound (0 = no spread, 360 = maximum spread)")]
        [SerializeField, ConditionalField("useAdvancedOptions", false, "use3DSound", false)]
        [Range(0f, 360f)]
        private float Spread;

        [FormerlySerializedAs("volumeRolloff")]
        [Tooltip("Volume rolloff mode for 3D sound")]
        [SerializeField, ConditionalField("useAdvancedOptions", false, "use3DSound", false)]
        private AudioRolloffMode VolumeRolloff = AudioRolloffMode.Logarithmic;

        [Tooltip("If true, the sound will always play fully and won't be interrupted.")]
        public bool ForcePlayToEnd = false;

        public AudioEventSfx()
        {
            Type = AudioType.Sfx;
        }

        public override void SetSource(AudioSource newSource, AudioMixerGroup mixer = null)
        {
            base.SetSource(newSource, mixer);

            if (Use3DSound)
            {
                Source.spatialBlend = 1.0f; // Fully 3D
                Source.minDistance = MinDistance;
                Source.maxDistance = Mathf.Clamp(MaxDistance, 1.01f, 1_000_000f); // Ensure maxDistance is within bounds
                Source.rolloffMode = VolumeRolloff;
            }
            else
            {
                Source.spatialBlend = 0.0f; // Fully 2D
            }
        }

        public float GetDopplerLevel()
        {
            return DopplerLevel;
        }

        public float GetSpread()
        {
            return Spread;
        }

        public AudioRolloffMode GetVolumeRolloff()
        {
            return VolumeRolloff;
        }

        public float GetMinDistance()
        {
            return MinDistance;
        }
    }
}
