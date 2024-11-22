using System;
using OCSFX.Attributes;
using OCSFX.Generics;
using OCSFX.Utility.Debug;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InputHandler: SingletonScriptableObject<InputHandler>
{
    [field: SerializeField] public InputActionAsset InputActions { get; private set; }
    [field: SerializeField, ReadOnly] public string[] InputActionMapNames { get; private set; }
    
    [field: Header("Gameplay Actions")]
    [field: SerializeField] public GameplayInputActions GameplayActions { get; private set; }
    // [field: SerializeField] public InputActionReference MoveActionRef { get; private set; }
    // [field: SerializeField] public InputActionReference CameraDragActionRef { get; private set; }
    // [field: SerializeField] public InputActionReference CameraZoomActionRef { get; private set; }
    // [field: SerializeField] public InputActionReference InteractActionRef { get; private set; }
    // [field: SerializeField] public InputActionReference PauseActionRef { get; private set; }
    
    [field: Header("UI Actions")]
    [field: SerializeField] public GameplayUIInputActions GameplayUIActions { get; private set; }

    [field: Header("Settings")]
    [field: SerializeField, Range(0.1f, 1f)] public float CameraZoomSensitivity { get; private set; } = 1f;
    
    [Header("Debug")]
    [SerializeField] private bool _showDebug;
    
    private InputActionMap _currentActionMap;

    public event Action<Vector2> OnMoveInput;
    public event Action<bool> OnDragCameraInput;
    public event Action<float> OnZoomInput;
    public event Action OnInteractInput;
    public event Action OnPauseInput;

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    private static void Init()
    {
        UnityEditor.EditorApplication.delayCall += () => Get();
    }
#endif
    
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
            gameplayUIActions.Resume.action.performed += OnPauseInputPerformed;
            
            EventBus.AddListener<PauseMenuController.UIEventParameters>(OnPauseMenuToggle);
            
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
            gameplayUIActions.Resume.action.performed -= OnPauseInputPerformed;
            
            EventBus.RemoveListener<PauseMenuController.UIEventParameters>(OnPauseMenuToggle);
            
            Application.quitting -= Deinitialize;
        }
    }

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

        OCSFXLogger.Log($"Pause menu toggle event received. Action: {info.Action}", Get(), Get()._showDebug);
    }

    private static void OnMoveInputPerformed(InputAction.CallbackContext context)
    {
        OCSFXLogger.Log($"Move input performed ({context.ReadValue<Vector2>()})", Get(), Get()._showDebug);
        
        var normalized = context.ReadValue<Vector2>().normalized;
        Get().OnMoveInput?.Invoke(normalized);
    }
    
    private static void OnDragCameraInputPerformed(InputAction.CallbackContext context)
    {
        var pressed = context.performed;
        OCSFXLogger.Log($"Drag camera input performed ({pressed})", Get(), Get()._showDebug);
        
        Get().OnDragCameraInput?.Invoke(pressed);
    }
    
    private static void OnZoomInputPerformed(InputAction.CallbackContext context)
    {
        if (context.canceled) return;
        
        var value = Mathf.Clamp(context.ReadValue<float>(), -1, 1);
        if (Mathf.Approximately(value, 0)) return;
        var scaledValue = value * Get().CameraZoomSensitivity;
        
        OCSFXLogger.Log($"Zoom input performed ({value}). Scaled value: ({scaledValue})", Get(), Get()._showDebug);
        
        Get().OnZoomInput?.Invoke(scaledValue);
    }
    
    private static void OnInteractInputPerformed(InputAction.CallbackContext context)
    {
        OCSFXLogger.Log($"Interact input performed", Get(), Get()._showDebug);
        
        Get().OnInteractInput?.Invoke();
    }
    
    private static void OnPauseInputPerformed(InputAction.CallbackContext obj)
    {
        OCSFXLogger.Log($"Pause input performed", Get(), Get()._showDebug);
        
        Get().OnPauseInput?.Invoke();
    }
    
    public void SetCurrentActionMap(string actionMapName)
    {
        var actionMap = InputActions.FindActionMap(actionMapName);
        if (actionMap == null)
        {
            OCSFXLogger.Log($"Action map {actionMapName} not found", this, _showDebug);
            return;
        }
        
        InputActions.Disable();
        
        _currentActionMap?.Disable();
        _currentActionMap = actionMap;
        _currentActionMap.Enable();
    }
    
    public void SetCurrentActionMap(InputActionMap actionMap)
    {
        if (actionMap == null)
        {
            OCSFXLogger.Log($"Action map is null", this, _showDebug);
            return;
        }
        
        InputActions.Disable();
        
        _currentActionMap?.Disable();
        _currentActionMap = actionMap;
        _currentActionMap.Enable();
    }

    private void OnValidate()
    {
        if (!InputActions)
        {
            InputActionMapNames = Array.Empty<string>();
            return;
        }
        
        var actionMaps = InputActions.actionMaps;
        InputActionMapNames = new string[actionMaps.Count];

        for (int i = 0; i < InputActions.actionMaps.Count; i++)
        {
            InputActionMapNames[i] = InputActions.actionMaps[i].name;
        }

        GameplayActions?.TrySetActionMapFromAsset(InputActions);
        GameplayUIActions?.TrySetActionMapFromAsset(InputActions);
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
    }

    [Serializable]
    public abstract class InputActionsCollection
    {
        [field: SerializeField] public string ActionMapName { get; private set; }
        public InputActionMap ActionMap { get; private set; }
        
        public bool TrySetActionMapFromAsset(InputActionAsset asset)
        {
            if (!asset)
            {
                OCSFXLogger.LogError($"[{nameof(InputActionsCollection)}] {nameof(TrySetActionMapFromAsset)}: " +
                                     $"Input action asset is null");
                return false;
            }
            var foundActionMap = asset.FindActionMap(ActionMapName);
            if (foundActionMap == null)
            {
                OCSFXLogger.LogError($"[{nameof(InputActionsCollection)}] {nameof(TrySetActionMapFromAsset)}: " +
                                     $"Action map {ActionMapName} not found in asset {asset.name}");
                return false;
            }
            
            ActionMap = foundActionMap;
            return true;
        }
        
        public void SetActionMap(InputActionMap actionMap)
        {
            // Set action map
            if (actionMap == ActionMap) return;
            
            ActionMap?.Disable();
            ActionMap = actionMap;
        }
    }
}
