using System.Collections.Generic;
using Runtime.Cameras;
using Runtime.Collectables;
using Runtime.SceneLoading;
using Runtime.UI;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using OCSFX.EZFMOD.Components;
using OCSFX.EZFMOD.Debug;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif //UNITY_EDITOR

namespace Runtime
{
    public class GameOff2024GameSettings : ScriptableObject
    {
        [field: Header("Player")]
        [field: SerializeField] public InputActionAsset InputActions { get; private set; }
        [field: SerializeField, Tag] public string PlayerTag { get; private set; } = "Player";
        [field: SerializeField, Expandable] public PlayerCharacter PlayerCharacterPrefab { get; private set; }
        [field: SerializeField, Expandable] public GameOff2024CameraControllerBase PlayerCameraPrefab { get; private set; }
        
        [field: Header("Effects")]
        [field: SerializeField, Expandable] public EZFMODAudioManager AudioManagerPrefab { get; private set; }
        [field: SerializeField, Expandable] public Volume PostProcessingVolumePrefab { get; private set; }
        
        [field: Header("UI")]
        [field: SerializeField, Expandable] public HudController HudPrefab { get; private set; }
        [field: SerializeField, Expandable] public PauseMenuController PauseMenuPrefab { get; private set; }
        [field: SerializeField, Expandable] public UIHoverDetector UIHoverDetectorPrefab { get; private set; }
        [field: SerializeField, Expandable] public Canvas UGUICanvasPrefab { get; private set; }
        [field: SerializeField, Expandable] public LoadingScreen LoadingScreenPrefab { get; private set; }
        [field: SerializeField, Expandable] public SceneLoadManager SceneLoadManagerPrefab { get; private set; }
        [field: SerializeField, Expandable] public ScreenFade ScreenFadePrefab { get; private set; }
        [field: SerializeField, Expandable] public ScoreUI ScoreUIPrefab { get; private set; }
        [field: SerializeField, Expandable] public HintManager HintManagerPrefab { get; private set; }
        
        [field: Header("Game")]
        [field: SerializeField, Expandable] public ItemInventory ItemInventoryPrefab { get; private set; }
        [field: SerializeField, Expandable] public CollectableData[] KeyCollectables { get; private set; }
        [SerializeField, Readonly] private int _totalOptionalCollectables = 15;
        [field: SerializeField, BuildSceneName] public string MainMenuSceneName = "MainMenu";
        [field: SerializeField, BuildSceneName] public string EndingSceneName = "Ending";
        [field: SerializeField, BuildSceneName] public string StartGameSceneName = "Game";
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
                (UGUICanvasPrefab, nameof(UGUICanvasPrefab)),
                (LoadingScreenPrefab, nameof(LoadingScreenPrefab)),
                (SceneLoadManagerPrefab, nameof(SceneLoadManagerPrefab)),
                (ScreenFadePrefab, nameof(ScreenFadePrefab)),
                (ItemInventoryPrefab, nameof(ItemInventoryPrefab)),
                (ScoreUIPrefab, nameof(ScoreUIPrefab)),
                (HintManagerPrefab, nameof(HintManagerPrefab)),
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
        
            
        protected static GameOff2024GameSettings _instance;
        public static GameOff2024GameSettings Get()
        {
            if (!_instance)
            {
                _instance = GetOrCreate();
            }

            return _instance;
        }
        
        private static GameOff2024GameSettings GetOrCreate()
        {
            var assetInstance = Resources.Load<GameOff2024GameSettings>(nameof(GameOff2024GameSettings));
            
#if UNITY_EDITOR
            if (assetInstance) return assetInstance;
            
            var assetPath = $"Assets/Resources/{nameof(GameOff2024GameSettings)}";
            
            EnsureDirectoryExists(assetPath);

            assetInstance = GetOrCreateScriptableObjectAsset<GameOff2024GameSettings>(
                assetPath, nameof(GameOff2024GameSettings), out var createdNew);

            if (createdNew)
            {
                OCSFXLogger.Log($"New instance of {nameof(GameOff2024GameSettings)} created at {assetPath}");
            }
#endif //UNITY_EDITOR

            return assetInstance;
        }
        
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void Init() => EditorApplication.delayCall += () => Get();

        private static void EnsureDirectoryExists(string directoryPath)
        {
            if (!directoryPath.EndsWith("/")) directoryPath += "/";

            var directoryName = Path.GetDirectoryName(directoryPath);

            if (Directory.Exists(directoryName)) return;

            if (directoryName == null) return;

            Directory.CreateDirectory(directoryName);
            AssetDatabase.Refresh();
        }

        private static T GetOrCreateScriptableObjectAsset<T>(string assetPath, string assetName, out bool createdNew) where T : ScriptableObject
        {
            if (string.IsNullOrWhiteSpace(assetPath) || string.IsNullOrWhiteSpace(assetName))
            {
                createdNew = false;
                return null;
            }

            createdNew = false;

            var fullPath = $"{assetPath}/{assetName}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<T>(fullPath);

            if (asset) return asset;

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, fullPath);
            AssetDatabase.SaveAssetIfDirty(asset);

            createdNew = true;

            return asset;
        }
#endif //UNITY_EDITOR
    }
}