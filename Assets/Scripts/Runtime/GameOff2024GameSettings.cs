using System.Collections.Generic;
using OCSFX.FMOD.Components;
using OCSFX.Generics;
using OCSFX.Utility.Debug;
using Runtime.Cameras;
using Runtime.Collectables;
using Runtime.Utility;
using Runtime.World;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace Runtime
{
    public class GameOff2024GameSettings : SingletonScriptableObject<GameOff2024GameSettings>
    {
        [field: Header("Player")]
        [field: SerializeField] public InputActionAsset InputActions { get; private set; }
        [field: SerializeField, TagField] public string PlayerTag { get; private set; } = "Player";
        [field: SerializeField, Expandable] public PlayerCharacter PlayerCharacterPrefab { get; private set; }
        [field: SerializeField, Expandable] public GameOff2024CameraControllerBase PlayerCameraPrefab { get; private set; }
        
        [field: Header("Effects")]
        [field: SerializeField, Expandable] public AudioManager AudioManagerPrefab { get; private set; }
        [field: SerializeField, Expandable] public Volume PostProcessingVolumePrefab { get; private set; }
        
        [field: Header("UI")]
        [field: SerializeField, Expandable] public HudController HudPrefab { get; private set; }
        [field: SerializeField, Expandable] public PauseMenuController PauseMenuPrefab { get; private set; }
        [field: SerializeField, Expandable] public UIHoverDetector UIHoverDetectorPrefab { get; private set; }
        
        [field: Header("Game")]
        [field: SerializeField, Expandable] public CollectableData[] KeyCollectables { get; private set; }
        [SerializeField, Readonly] private int _totalOptionalCollectables = 15;
        // [field: SerializeField, Expandable] public SceneLoader LevelPreloaderPrefab { get; private set; }

        [field: SerializeField, BuildSceneName]
        public string[] ExcludeScenesFromInitialization = new string[0];

        public int TotalOptionalCollectables
        {
            get
            {
                _totalOptionalCollectables = _optionalCollectableIDs.Count;
                return _totalOptionalCollectables;
            }
        }

        private readonly HashSet<string> _optionalCollectableIDs = new HashSet<string>();
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void Init() => UnityEditor.EditorApplication.delayCall += () => Get();
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void RuntimeInit()
        {
            var instance = Get();
            if (!instance) return;
            
            instance._optionalCollectableIDs.Clear();
            
            instance.ValidateFields();
        }

        public bool RegisterOptionalCollectable(GameOff2024Collectable collectable)
        {
            var added = collectable && _optionalCollectableIDs.Add(collectable.ID);
            if (added) _totalOptionalCollectables = _optionalCollectableIDs.Count;
            
            return added;
        }

        private void ValidateFields()
        {
            var fields = new (object field, string name)[]
            {
                (InputActions, nameof(InputActions)),
                (PlayerTag, nameof(PlayerTag)),
                (PlayerCharacterPrefab, nameof(PlayerCharacterPrefab)),
                (AudioManagerPrefab, nameof(AudioManagerPrefab)),
                (PostProcessingVolumePrefab, nameof(PostProcessingVolumePrefab)),
                (HudPrefab, nameof(HudPrefab)),
                (PauseMenuPrefab, nameof(PauseMenuPrefab)),
                (UIHoverDetectorPrefab, nameof(UIHoverDetectorPrefab)),
                // (LevelPreloaderPrefab, nameof(LevelPreloaderPrefab))
            };

            foreach (var (field, fieldName) in fields)
            {
                if (field is null || (field is string str && string.IsNullOrEmpty(str)))
                {
                    WarnOfMissingField(fieldName);
                }
            }

            if (_showDebug) OCSFXLogger.Log($"[{nameof(GameOff2024GameSettings)}] All fields are valid.", this);
        }
        
        private void WarnOfMissingField(string fieldName)
        {
            OCSFXLogger.LogWarning($"[{nameof(GameOff2024GameSettings)}] {fieldName} is unassigned. This may cause problems during the game.", this);
        }
    }
}