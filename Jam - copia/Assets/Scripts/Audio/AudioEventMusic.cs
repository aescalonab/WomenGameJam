using UnityEngine;
using UnityEngine.Audio;

namespace Code.Audio
{
    [System.Serializable, CreateAssetMenu(menuName = "Audio Event/Music")]
    public class AudioEventMusic : AudioEvent
    {
        public AudioEventMusic()
        {
            this.Type = AudioType.Music;
        }

        public override void SetSource(AudioSource newSource, AudioMixerGroup mixer = null)
        {
            base.SetSource(newSource, mixer);

            Source.spatialBlend = 0.0f; // Always 2D for music
        }
    }
}
