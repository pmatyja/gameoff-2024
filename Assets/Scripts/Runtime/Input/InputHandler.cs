using System;
using OCSFX.EZFMOD.Debug;
using Runtime.UI;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class InputHandler: ScriptableObject
{
    [field: SerializeField] public InputActionAsset InputActions { get; private set; }
    
    [field: Header("Gameplay Actions")]
    [field: SerializeField] public GameplayInputActions GameplayActions { get; private set; }
    
    [field: Header("UI Actions")]
    [field: SerializeField] public GameplayUIInputActions GameplayUIActions { get; private set; }
    [field: SerializeField] public FrontEndUIInputActions FrontEndUIActions { get; private set; }
    
    [field: Header("Cutscene Actions")]
    [field: SerializeField] public CutsceneInputActions CutsceneActions { get; private set; }

    [field: Header("Settings")]
    [field: SerializeField, Range(0.1f, 1f)] public float CameraZoomSensitivity { get; private set; } = 1f;
    
    [Header("Debug")]
    [SerializeField] private bool _showDebug;
    
    private InputActionMap _currentActionMap;

    public event Action<Vector2> OnGameplayMoveInput;
    public event Action<bool> OnGameplayDragCameraInput;
    public event Action<float> OnGameplayCameraZoomInput;
    public event Action OnGameplayInteractInput;
    public event Action OnGameplayPauseInput;
    public event Action OnCutsceneSkipInput;
    
    public event Action OnUIGameplayResumeInput;
    // public event Action OnUIGameplayMoveInput;
    //
    // public event Action OnFrontEndUIMoveInput;
    // public event Action OnFrontEndUIConfirmInput;
    // public event Action OnFrontEndUICancelInput;
    
    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        Get().InputActions.Enable();
        
        SetEventBindings(true);
        
        OCSFXLogger.Log($"{nameof(InputHandler)} initialized", Get(), Get()._showDebug);
    }
    
    private static void Deinitialize()
    {
        if (!Get()) return;
        
        SetEventBindings(false);
        
        Get().InputActions.Disable();
    }
    
    private static void SetEventBindings(bool bind)
    {
        if (bind)
        {
            var gameplayActions = Get().GameplayActions;
            gameplayActions.Move.action.performed += OnMoveInputPerformed;
            gameplayActions.Move.action.canceled += OnMoveInputPerformed;
            gameplayActions.Interact.action.performed += OnInteractInputPerformed;
            gameplayActions.CameraDrag.action.performed += OnDragCameraInputPerformed;
            gameplayActions.CameraDrag.action.canceled += OnDragCameraInputPerformed;
            gameplayActions.CameraZoom.action.performed += OnZoomInputPerformed;
            gameplayActions.Pause.action.performed += OnPauseInputPerformed;
            
            var gameplayUIActions = Get().GameplayUIActions;
            gameplayUIActions.Resume.action.performed += OnResumeInputPerformed;
            
            var cutsceneActions = Get().CutsceneActions;
            cutsceneActions.Skip.action.performed += OnCutsceneSkipInputPerformed;
            
            EventBus.AddListener<PauseMenuController.UIEventParameters>(OnPauseMenuToggle);

            GameOff2024VideoPlayerScreen.OnScreenEnabled += OnVideoScreenEnabled;
            GameOff2024VideoPlayerScreen.OnScreenDisabled += OnVideoScreenDisabled;
            
            Application.quitting += Deinitialize;
        }
        else
        {
            var gameplayActions = Get().GameplayActions;
            gameplayActions.Move.action.performed -= OnMoveInputPerformed;
            gameplayActions.Move.action.canceled -= OnMoveInputPerformed;
            gameplayActions.Interact.action.performed -= OnInteractInputPerformed;
            gameplayActions.CameraDrag.action.performed -= OnDragCameraInputPerformed;
            gameplayActions.CameraDrag.action.canceled -= OnDragCameraInputPerformed;
            gameplayActions.CameraZoom.action.performed -= OnZoomInputPerformed;
            gameplayActions.Pause.action.performed -= OnPauseInputPerformed;
            
            var gameplayUIActions = Get().GameplayUIActions;
            gameplayUIActions.Resume.action.performed -= OnResumeInputPerformed;
            
            var cutsceneActions = Get().CutsceneActions;
            cutsceneActions.Skip.action.performed -= OnCutsceneSkipInputPerformed;
            
            EventBus.RemoveListener<PauseMenuController.UIEventParameters>(OnPauseMenuToggle);
            
            GameOff2024VideoPlayerScreen.OnScreenEnabled -= OnVideoScreenEnabled;
            GameOff2024VideoPlayerScreen.OnScreenDisabled -= OnVideoScreenDisabled;
            
            Application.quitting -= Deinitialize;
        }
    }

    private static void OnVideoScreenEnabled()
    {
        Get().SetCurrentActionMap(Get().CutsceneActions.ActionMap);

        OCSFXLogger.Log($"[{nameof(InputHandler)}] {nameof(OnVideoScreenEnabled)}", Get(), Get()._showDebug);
    }

    private static void OnVideoScreenDisabled()
    {
        Get().SetCurrentActionMap(Get().GameplayActions.ActionMap);
        
        OCSFXLogger.Log($"[{nameof(InputHandler)}] {nameof(OnVideoScreenDisabled)}", Get(), Get()._showDebug);
    }

    public static void DisableInput() => Get().InputActions.Disable();

    public static void EnableInput() => Get().InputActions.Enable();

    private static void OnPauseMenuToggle(object sender, PauseMenuController.UIEventParameters info)
    {
        switch (info.Action)
        {
            case PauseMenuController.UIAction.OpenMenu:
                Get().SetCurrentActionMap(Get().GameplayUIActions.ActionMap);
                break;
            default:
            case PauseMenuController.UIAction.CloseMenu:
                Get().SetCurrentActionMap(Get().GameplayActions.ActionMap);
                break;
        }

        OCSFXLogger.Log($"[{nameof(InputHandler)}] Pause menu toggle event received. Action: {info.Action}", Get(), Get()._showDebug);
    }

    private static void OnMoveInputPerformed(InputAction.CallbackContext context)
    {
        OCSFXLogger.Log($"[{nameof(InputHandler)}] Move input performed ({context.ReadValue<Vector2>()})", Get(), Get()._showDebug);
        
        var normalized = context.ReadValue<Vector2>().normalized;
        Get().OnGameplayMoveInput?.Invoke(normalized);
    }
    
    private static void OnDragCameraInputPerformed(InputAction.CallbackContext context)
    {
        var pressed = context.performed;
        OCSFXLogger.Log($"[{nameof(InputHandler)}] Drag camera input performed ({pressed})", Get(), Get()._showDebug);
        
        Get().OnGameplayDragCameraInput?.Invoke(pressed);
    }
    
    private static void OnZoomInputPerformed(InputAction.CallbackContext context)
    {
        if (context.canceled) return;
        
        var value = Mathf.Clamp(context.ReadValue<float>(), -1, 1);
        if (Mathf.Approximately(value, 0)) return;
        var scaledValue = value * Get().CameraZoomSensitivity;
        
        OCSFXLogger.Log($"[{nameof(InputHandler)}] Zoom input performed ({value}). Scaled value: ({scaledValue})", Get(), Get()._showDebug);
        
        Get().OnGameplayCameraZoomInput?.Invoke(scaledValue);
    }
    
    private static void OnInteractInputPerformed(InputAction.CallbackContext context)
    {
        OCSFXLogger.Log($"[{nameof(InputHandler)}] Interact input performed", Get(), Get()._showDebug);
        
        Get().OnGameplayInteractInput?.Invoke();
    }
    
    private static void OnPauseInputPerformed(InputAction.CallbackContext context)
    {
        OCSFXLogger.Log($"[{nameof(InputHandler)}] Pause input performed", Get(), Get()._showDebug);
        
        Get().OnGameplayPauseInput?.Invoke();
    }
    
    private static void OnResumeInputPerformed(InputAction.CallbackContext context)
    {
        OCSFXLogger.Log($"[{nameof(InputHandler)}] Resume input performed", Get(), Get()._showDebug);
        
        Get().OnUIGameplayResumeInput?.Invoke();
    }
    
    private static void OnCutsceneSkipInputPerformed(InputAction.CallbackContext context)
    {
        OCSFXLogger.Log($"[{nameof(InputHandler)}] Cutscene skip input performed", Get(), Get()._showDebug);
        
        Get().OnCutsceneSkipInput?.Invoke();
    }
    
    public void SetCurrentActionMap(InputActionMap actionMap)
    {
        if (actionMap == null)
        {
            OCSFXLogger.Log($"[{nameof(InputHandler)}] Action map is null", this, _showDebug);
            return;
        }
        
        var validatedActionMap = InputActions.FindActionMap(actionMap.id);
        
        if (validatedActionMap == null)
        {
            OCSFXLogger.Log($"[{nameof(InputHandler)}] Action map ({actionMap.name}) not found in input actions asset ({InputActions.name})", this, _showDebug);
            return;
        }

        if (validatedActionMap == _currentActionMap) return;
        
        InputActions.Disable();

        if (_currentActionMap != null)
        {
            OCSFXLogger.Log($"[{nameof(InputHandler)}] Disabling current action map ({_currentActionMap.name})", this, _showDebug);   
        }
        
        _currentActionMap?.Disable();
        _currentActionMap = validatedActionMap;
        _currentActionMap.Enable();
        
        OCSFXLogger.Log($"[{nameof(InputHandler)}] Set and enable current action map ({_currentActionMap.name})", this, _showDebug);
    }
    
    public void SetCurrentActionMap(InputActionsCollection actionsCollection)
    {
        SetCurrentActionMap(actionsCollection.ActionMap);
    }
    
    public void SetCurrentActionMap(InputActionMapRef actionMapRef)
    {
        SetCurrentActionMap(actionMapRef.Map);
    }
    
    public void SetCurrentActionMap(string actionMapName)
    {
        var actionMap = InputActions.FindActionMap(actionMapName);
        if (actionMap == null)
        {
            OCSFXLogger.Log($"[{nameof(InputHandler)}] Action map ({actionMapName}) not found in input actions asset ({InputActions.name})", this, _showDebug);
            return;
        }
        
        SetCurrentActionMap(actionMap);
    }

    [Serializable]
    public class GameplayInputActions : InputActionsCollection
    {
        [field:SerializeField] public InputActionReference Move { get; private set; }
        [field:SerializeField] public InputActionReference CameraDrag { get; private set; }
        [field:SerializeField] public InputActionReference CameraZoom { get; private set; }
        [field:SerializeField] public InputActionReference Interact { get; private set; }
        [field:SerializeField] public InputActionReference Pause { get; private set; }
    }
    
    [Serializable]
    public class GameplayUIInputActions : InputActionsCollection
    {
        [field: SerializeField] public InputActionReference Resume { get; private set; }
        [field: SerializeField] public InputActionReference Confirm { get; private set; }
        [field: SerializeField] public InputActionReference Move { get; private set; }
    }
    
    [Serializable]
    public class FrontEndUIInputActions : InputActionsCollection
    {
        [field: SerializeField] public InputActionReference Confirm { get; private set; }
        [field: SerializeField] public InputActionReference Cancel { get; private set; }
        [field: SerializeField] public InputActionReference Move { get; private set; }
    }
    
    [Serializable]
    public class CutsceneInputActions : InputActionsCollection
    {
        [field: SerializeField] public InputActionReference Skip { get; private set; }
    }

    [Serializable]
    public abstract class InputActionsCollection
    {
        [field: SerializeField] public InputActionMapRef ActionMapRef { get; private set; }
        public InputActionMap ActionMap => ActionMapRef.Map;
    }
    
    protected static InputHandler _instance;
    public static InputHandler Get()
    {
        if (!_instance)
        {
            _instance = GetOrCreate();
        }

        return _instance;
    }
    
    private static InputHandler GetOrCreate()
    {
        var resourcePath = $"{nameof(InputHandler)}/{nameof(InputHandler)}";
        var assetInstance = Resources.Load<InputHandler>(resourcePath);
        
#if UNITY_EDITOR
        if (assetInstance) return assetInstance;
        
        var assetPath = $"Assets/Resources/{nameof(InputHandler)}";
        
        EnsureDirectoryExists(assetPath);

        assetInstance = GetOrCreateScriptableObjectAsset<InputHandler>(
            assetPath, nameof(InputHandler), out var createdNew);

        if (createdNew)
        {
            OCSFXLogger.Log($"New instance of {nameof(InputHandler)} created at {assetPath}");
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