using OCSFX.FMOD.Components;
using OCSFX.Generics;
using OCSFX.Utility.Debug;
using Runtime.Cameras;
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
        [field: SerializeField, Expandable] public UserInterface UserInterfacePrefab { get; private set; }
        [field: SerializeField, Expandable] public PauseMenuController PauseMenuPrefab { get; private set; }
        [field: SerializeField, Expandable] public UIHoverDetector UIHoverDetectorPrefab { get; private set; }
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void Init() => UnityEditor.EditorApplication.delayCall += () => Get();
#endif

        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInit()
        {
            Get().ValidateFields();
        }

        private void ValidateFields()
        {
            if (!InputActions) WarnOfMissingField(nameof(InputActions));
            if (string.IsNullOrEmpty(PlayerTag)) WarnOfMissingField(nameof(PlayerTag));
            if (!PlayerCharacterPrefab) WarnOfMissingField(nameof(PlayerCharacterPrefab));
            if (!AudioManagerPrefab) WarnOfMissingField(nameof(AudioManagerPrefab));
            if (!PostProcessingVolumePrefab) WarnOfMissingField(nameof(PostProcessingVolumePrefab));
            
            if (_showDebug) OCSFXLogger.Log($"[{nameof(GameOff2024GameSettings)}] All fields are valid.", this);
        }
        
        private void WarnOfMissingField(string fieldName)
        {
            OCSFXLogger.LogWarning($"[{nameof(GameOff2024GameSettings)}] {fieldName} is unassigned. This may cause problems during the game.", this);
        }
    }
}