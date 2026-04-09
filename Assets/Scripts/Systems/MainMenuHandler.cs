using UnityEngine;
using UnityEngine.UI;

namespace Systems
{
    public class MainMenuHandler : MonoBehaviour
    {
        public GameObject mainMenu;
        public GameObject settingsMenu;
        public GameObject creditsMenu;
        
        public Slider mainVolumeSlider;
        public Slider sfxVolumeSlider;
        public Slider musicVolumeSlider;
        
        private SoundManager soundManager;

        private void Start()
        {
            if (PlayerPrefs.HasKey("CurrentObjectiveIndex"))
            {
                PlayerPrefs.DeleteKey("CurrentObjectiveIndex");
            }
            
            soundManager = FindFirstObjectByType<SoundManager>();
            if (soundManager)
            {
                mainVolumeSlider.value = soundManager.GetSoundVolume(SoundManager.MainVolume);
                sfxVolumeSlider.value = soundManager.GetSoundVolume(SoundManager.SoundVolume);
                musicVolumeSlider.value = soundManager.GetSoundVolume(SoundManager.MusicVolume);
            }
            else
            {
                Debug.LogError("No sound manager found");
            }
            
            GoToMainMenu();
        }
        
        public void GoToMainMenu()
        {
            mainMenu.SetActive(true);
            settingsMenu.SetActive(false);
            creditsMenu.SetActive(false);
        }

        public void GoToSettingsMenu()
        {
            settingsMenu.SetActive(true);
            mainMenu.SetActive(false);
        }

        public void GoToCreditsMenu()
        {
            creditsMenu.SetActive(true);
            mainMenu.SetActive(false);
        }

        public void Quit()
        {
            Application.Quit();
        }
        
        public void OnMainVolumeChanged()
        {
            if (!soundManager) Debug.LogError("No sound manager found");
            
            soundManager.OnChangeVolume(SoundManager.MainVolume, mainVolumeSlider.value);
        }

        public void OnSfxVolumeChanged()
        {
            if (!soundManager) Debug.LogError("No sound manager found");
            
            soundManager.OnChangeVolume(SoundManager.SoundVolume, sfxVolumeSlider.value);
        }

        public void OnMusicVolumeChanged()
        {
            if (!soundManager) Debug.LogError("No sound manager found");
            
            soundManager.OnChangeVolume(SoundManager.MusicVolume, musicVolumeSlider.value);
        }
    }
}
