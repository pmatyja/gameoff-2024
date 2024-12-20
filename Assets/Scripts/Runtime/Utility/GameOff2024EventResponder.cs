﻿using System;
using FMODUnity;
using OCSFX.Utility.Debug;
using Runtime.Collectables;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Runtime.Utility
{
    public class GameOff2024EventResponder : MonoBehaviour
    {
        [SerializeField] private bool _showDebug;
        
        [SerializeField] private UnityEvent _onBasicItemCollected;
        [SerializeField] private UnityEvent _onKeyItemCollected;
        [SerializeField] private UnityEvent<int> _onKeysCollectedChanged;
        [SerializeField] private UnityEvent _onGameSettingsChanged;
        [SerializeField] private UnityEvent<string, float> _onGameSettingsAudioValueChanged;
        [SerializeField] private AudioSettingIdConversion[] _audioSettingIdConversions;
        [Space]
        [SerializeField] private UnityEvent _onPauseMenuOpen;
        [SerializeField] private UnityEvent _onPauseMenuClose;

        [Space]
        [SerializeField] private FrontEndButtonEvent[] _frontEndButtonEvents = new FrontEndButtonEvent[]
        {
            new FrontEndButtonEvent { ButtonName = "Menu-Start", OnButtonPressed = new UnityEvent() },
            new FrontEndButtonEvent { ButtonName = "Menu-Credits", OnButtonPressed = new UnityEvent() },
            new FrontEndButtonEvent { ButtonName = "Menu-Back", OnButtonPressed = new UnityEvent() },
        };

        private void OnEnable()
        {
            EventBus.AddListener<CollectableEventParameters>(OnCollectableCollected);
            EventBus.AddListener<GameSettingsChangedEventParameters>(OnGameSettingsChanged);
            EventBus.AddListener<PauseMenuController.UIEventParameters>(OnPauseMenuControllerUIEvent);
            EventBus.AddListener<GameSceneEventParameters>(OnLavgineStartGameButtonPressed);
            EventBus.AddListener<MouseClickEvent>(OnLavgineMouseClickEvent);

            ItemInventory.OnKeyItemsCollectedChanged += OnKeyItemsCollectedChanged;
        }

        private void OnDisable()
        {
            EventBus.RemoveListener<CollectableEventParameters>(OnCollectableCollected);
            EventBus.RemoveListener<GameSettingsChangedEventParameters>(OnGameSettingsChanged);
            EventBus.RemoveListener<PauseMenuController.UIEventParameters>(OnPauseMenuControllerUIEvent);
            EventBus.RemoveListener<GameSceneEventParameters>(OnLavgineStartGameButtonPressed);
            EventBus.RemoveListener<MouseClickEvent>(OnLavgineMouseClickEvent);
            
            ItemInventory.OnKeyItemsCollectedChanged -= OnKeyItemsCollectedChanged;
        }

        private void OnLavgineMouseClickEvent(object sender, MouseClickEvent info)
        {
            Debug.Log($"{info.Element.name} was clicked", this);
            
            HandleFrontEndMenuButtonPressed(info.Element.name);
        }

        private void OnLavgineStartGameButtonPressed(object sender, GameSceneEventParameters info)
        {
            // _onFrontEndStartGameButtonPressed?.Invoke();
        }
        
        private void HandleFrontEndMenuButtonPressed(string buttonName)
        {
            foreach (var frontEndButtonEvent in _frontEndButtonEvents)
            {
                if (frontEndButtonEvent.ButtonName == buttonName)
                {
                    frontEndButtonEvent.OnButtonPressed?.Invoke();
                    return;
                }
            }
        }

        private void OnKeyItemsCollectedChanged(int keyItemCount)
        {
            _onKeysCollectedChanged?.Invoke(keyItemCount);
        }

        private void OnCollectableCollected(object sender, CollectableEventParameters info)
        {
            var asGameOff2024Collectable = (GameOff2024Collectable)info.Collectable;
            if (!asGameOff2024Collectable)
            {
                _onBasicItemCollected?.Invoke();
                return;
            }
            
            var data = asGameOff2024Collectable.Data;
            if (data && data.IsUnique)
            {
                _onKeyItemCollected?.Invoke();
            }
            else
            {
                _onBasicItemCollected?.Invoke();   
            }
            
            OCSFXLogger.Log($"[{nameof(GameOff2024EventResponder)}] {nameof(OnCollectableCollected)}", this, _showDebug);
        }
        
        private void OnGameSettingsChanged(object sender, GameSettingsChangedEventParameters info)
        {
            _onGameSettingsChanged?.Invoke();
            
            if (info.Subsystem == GameSettingsSubsystem.Audio)
            {
                var convertedId = GetVolumeSettingsId(info.Id);
                
                if (convertedId == null)
                {
                    OCSFXLogger.LogWarning($"[{nameof(GameOff2024EventResponder)}] {nameof(OnGameSettingsChanged)} : " +
                                    $"No {nameof(AudioSettingIdConversion)} found for {info.Id}", this, _showDebug);
                    return;
                }
                
                OCSFXLogger.Log($"[{nameof(GameOff2024EventResponder)}] {nameof(OnGameSettingsChanged)} :" +
                                $" Volume Setting changed : " +
                                $"(Id: {info.Id}, Value: {info.Value}, VolumeSetting: {convertedId})", this, _showDebug);
                
                var floatValue = (float)(object)info.Value;
                _onGameSettingsAudioValueChanged?.Invoke(convertedId, floatValue);
            }
        }
        
        private void OnPauseMenuControllerUIEvent(object sender, PauseMenuController.UIEventParameters info)
        {
            switch (info.Action)
            {
                case PauseMenuController.UIAction.OpenMenu:
                    OCSFXLogger.Log($"[{nameof(GameOff2024EventResponder)}] {nameof(OnPauseMenuControllerUIEvent)} : Open Menu", this, _showDebug);
                    _onPauseMenuOpen?.Invoke();
                    break;
                default:
                case PauseMenuController.UIAction.CloseMenu:
                    OCSFXLogger.Log($"[{nameof(GameOff2024EventResponder)}] {nameof(OnPauseMenuControllerUIEvent)} : Close Menu", this, _showDebug);
                    _onPauseMenuClose?.Invoke();
                    break;
            }
        }
        
        private string GetVolumeSettingsId(string gameSettingsId)
        {
            foreach (var conversion in _audioSettingIdConversions)
            {
                var volumeSettingsId = conversion.GetVolumeSettingsId(gameSettingsId);
                if (volumeSettingsId != null)
                {
                    return volumeSettingsId;
                }
            }
            
            return null;
        }

        [Serializable]
        private class AudioSettingIdConversion
        {
            public string GameSettingsId;
            [ParamRef] public string VolumeSettingsId;
            
            public AudioSettingIdConversion(string gameSettingsId, string volumeSettingsId)
            {
                GameSettingsId = gameSettingsId;
                VolumeSettingsId = volumeSettingsId;
            }
            
            public string GetVolumeSettingsId(string gameSettingsId)
            {
                return GameSettingsId == gameSettingsId ? VolumeSettingsId : null;
            }
            
            public string GetGameSettingsId(string volumeSettingsId)
            {
                return VolumeSettingsId == volumeSettingsId ? GameSettingsId : null;
            }
        }
    }
    
    [Serializable]
    public class FrontEndButtonEvent
    {
        public string ButtonName;
        public UnityEvent OnButtonPressed;
    }
}