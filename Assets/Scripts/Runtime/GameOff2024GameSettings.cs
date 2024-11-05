using OCSFX.Generics;
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
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void Init() => UnityEditor.EditorApplication.delayCall += () => Get();
#endif
        
    }
}