using FMOD.Studio;
using FMODUnity;
using GameSetting;
using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class VolumeOptionController : MonoBehaviour
    {
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider voiceSlider;
        [SerializeField] private Slider ambientSlider;
        [SerializeField] private Slider systemSlider;
        
        private VCA master;
        private VCA bgm;
        private VCA sfx;
        private VCA voice;
        private VCA ambient;
        private VCA system;
        
        private void Start()
        {
            master = RuntimeManager.GetVCA("vca:/Master");
            bgm = RuntimeManager.GetVCA("vca:/BGM");
            sfx = RuntimeManager.GetVCA("vca:/SFX");
            voice = RuntimeManager.GetVCA("vca:/Voice");
            ambient = RuntimeManager.GetVCA("vca:/Ambient");
            system = RuntimeManager.GetVCA("vca:/System");

            float masterVolume;
            float bgmVolume;
            float sfxVolume;
            float voiceVolume;
            float ambientVolume;
            float systemVolume;
            
            if (GameController.Instance != null && GameController.Instance.gameSettingsController != null)
            {
                var settings = GameController.Instance.gameSettingsController.gameSettings;
                masterVolume = settings.masterVolume;
                bgmVolume = settings.bgmVolume;
                sfxVolume = settings.sfxVolume;
                voiceVolume = settings.voiceVolume;
                ambientVolume = settings.ambientVolume;
                systemVolume = settings.systemVolume;
            }
            else
            {
                masterVolume = master.getVolume(out masterVolume) == FMOD.RESULT.OK ? masterVolume : 1.0f;
                bgmVolume = bgm.getVolume(out bgmVolume) == FMOD.RESULT.OK ? bgmVolume : 1.0f;
                sfxVolume = sfx.getVolume(out sfxVolume) == FMOD.RESULT.OK ? sfxVolume : 1.0f;
                voiceVolume = voice.getVolume(out voiceVolume) == FMOD.RESULT.OK ? voiceVolume : 1.0f;
                ambientVolume = ambient.getVolume(out ambientVolume) == FMOD.RESULT.OK ? ambientVolume : 1.0f;
                systemVolume = system.getVolume(out systemVolume) == FMOD.RESULT.OK ? systemVolume : 1.0f;
            }

            masterSlider.value = masterVolume;
            bgmSlider.value = bgmVolume;
            sfxSlider.value = sfxVolume;
            voiceSlider.value = voiceVolume;
            ambientSlider.value = ambientVolume;
            systemSlider.value = systemVolume;

            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            voiceSlider.onValueChanged.AddListener(SetVoiceVolume);
            ambientSlider.onValueChanged.AddListener(SetAmbientVolume);
            systemSlider.onValueChanged.AddListener(SetSystemVolume);
        }
        
        private void SetMasterVolume(float volume)
        {
            master.setVolume(volume);
            GameController.Instance.gameSettingsController.gameSettings.masterVolume = volume;
        }
        
        private void SetBGMVolume(float volume)
        {
            bgm.setVolume(volume);
            GameController.Instance.gameSettingsController.gameSettings.bgmVolume = volume;
        }
        
        private void SetSFXVolume(float volume)
        {
            sfx.setVolume(volume);
            GameController.Instance.gameSettingsController.gameSettings.sfxVolume = volume;
        }
        
        private void SetVoiceVolume(float volume)
        {
            voice.setVolume(volume);
            GameController.Instance.gameSettingsController.gameSettings.voiceVolume = volume;
        }
        
        private void SetAmbientVolume(float volume)
        {
            ambient.setVolume(volume);
            GameController.Instance.gameSettingsController.gameSettings.ambientVolume = volume;
        }
        
        private void SetSystemVolume(float volume)
        {
            system.setVolume(volume);
            GameController.Instance.gameSettingsController.gameSettings.systemVolume = volume;
        }
    }
}