using OCSFX.FMOD.Components;
using OCSFX.Generics;
using OCSFX.Utility.Debug;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime
{
    public class GameOff2024GameSettings : SingletonScriptableObject<GameOff2024GameSettings>
    {
        [field: Header(nameof(GameOff2024GameSettings))]
        [field: SerializeField] public InputActionAsset InputActions { get; private set; }
        [field: SerializeField, TagField] public string PlayerTag { get; private set; } = "Player";
        
        [field: Header("Audio")]
        [field: SerializeField, Expandable] public AudioManager AudioManagerPrefab { get; private set; }
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;

        public static AudioManager AudioManager;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void Init() => UnityEditor.EditorApplication.delayCall += () => Get();
#endif

        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInit()
        {
            var audioManagerPrefabRef = Get().AudioManagerPrefab;
            
            AudioManager = audioManagerPrefabRef ? Instantiate(audioManagerPrefabRef) : FindFirstObjectByType<AudioManager>();
            
            if (!AudioManager)
            {
                OCSFXLogger.LogError($"No {nameof(AudioManager)} found in the scene or in {nameof(GameOff2024GameSettings)}.");
            }
        }
        
    }
}