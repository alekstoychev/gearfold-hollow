using UnityEngine;
using UnityEngine.Audio;

namespace Systems
{
    public class SoundManager : MonoBehaviour
    {
        public const string MainVolume = "MainVolume";
        public const string SoundVolume = "SFXVolume";
        public const string MusicVolume = "MusicVolume";
        
        public AudioMixer mixer;
        public AudioSource musicSource;

        private void Start()
        {
            if (mixer)
            {
                mixer.SetFloat(MainVolume, Mathf.Log10(GetSoundVolume(MainVolume)) * 20);
                mixer.SetFloat(SoundVolume, Mathf.Log10(GetSoundVolume(SoundVolume)) * 20);
                mixer.SetFloat(MusicVolume, Mathf.Log10(GetSoundVolume(MusicVolume)) * 20);
            }
            else
            {
                Debug.LogError("No AudioMixer found!");
            }
            
            musicSource?.Play();
        }

        public void OnChangeVolume(string soundName, float value)
        {
            PlayerPrefs.SetFloat(soundName, value);
            
            mixer.SetFloat(soundName, Mathf.Log10(value) * 20);
        }

        public float GetSoundVolume(string soundName)
        {
            if (PlayerPrefs.HasKey(soundName))
            {
                return PlayerPrefs.GetFloat(soundName);
            }

            return 1;
        }

        public void SavePrefs()
        {
            PlayerPrefs.Save();
        }
    }
}
