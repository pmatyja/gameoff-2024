using OCSFX.EZFMOD.ScriptableObjects;
using Runtime.UI;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.Audio
{
    public class GameOff2024PauseMenuAudioSetter : MonoBehaviour
    {
        [SerializeField] private EZFMODVolumeSettingsAudioDataSO _volumeSettingsAudioData;
        [SerializeField] private GameOff2024EventResponder _gameOff2024EventResponder;
        
        private PauseMenuController _pauseMenuController;


        private void OnEnable()
        {
            if (!_volumeSettingsAudioData) return;
            
            _volumeSettingsAudioData.OnAudioPlayerPrefsLoaded += OnAudioPlayerPrefsLoaded;
        }
        
        private void OnDisable()
        {
            if (!_volumeSettingsAudioData) return;
            
            _volumeSettingsAudioData.OnAudioPlayerPrefsLoaded -= OnAudioPlayerPrefsLoaded;
        }

        private void Start()
        {
            if (!_pauseMenuController)
            {
                _pauseMenuController = GameOff2024Statics.GetPauseMenuController();
            }
        }

        private void OnAudioPlayerPrefsLoaded(EZFMODVolumeSettingsAudioDataSO.AudioPlayerPrefs audioPlayerPrefs)
        {
            if (!_pauseMenuController) return;
            if (!_gameOff2024EventResponder) return;
            
            _pauseMenuController.Master = audioPlayerPrefs.GetValue(
                _gameOff2024EventResponder.GetVolumeSettingsId(nameof(_pauseMenuController.Master)));
            _pauseMenuController.Sfx = audioPlayerPrefs.GetValue(
                _gameOff2024EventResponder.GetVolumeSettingsId(nameof(_pauseMenuController.Sfx)));
            _pauseMenuController.Music = audioPlayerPrefs.GetValue(
                _gameOff2024EventResponder.GetVolumeSettingsId(nameof(_pauseMenuController.Music)));
            _pauseMenuController.Ambient = audioPlayerPrefs.GetValue(
                _gameOff2024EventResponder.GetVolumeSettingsId(nameof(_pauseMenuController.Ambient)));
            _pauseMenuController.Voice = audioPlayerPrefs.GetValue(
                _gameOff2024EventResponder.GetVolumeSettingsId(nameof(_pauseMenuController.Voice)));
        }
    }
}