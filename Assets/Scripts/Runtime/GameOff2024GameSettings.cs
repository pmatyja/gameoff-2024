using OCSFX.Generics;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameOff2024
{
    public class GameOff2024GameSettings : SingletonScriptableObject<GameOff2024GameSettings>
    {
        [field: Header(nameof(GameOff2024GameSettings))]
        [field: Header("Input")]
        [field: SerializeField] public InputActionAsset InputActions { get; private set; }
        [field: SerializeField] public InputActionReference MoveAction { get; private set; }
        
        [field: Header("Tags")]
        [field: SerializeField, TagField] public string PlayerTag { get; private set; } = "Player";
        [field: SerializeField] public string ProjectName { get; private set; } = "GameOff2024";
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void Init() => UnityEditor.EditorApplication.delayCall += () => Get();
#endif
        
    }
}