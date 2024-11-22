using System;
using System.Linq;
using OCSFX.Attributes;
using OCSFX.Generics;
using OCSFX.Utility.Debug;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InputHandler: SingletonScriptableObject<InputHandler>
{
    [field: SerializeField] public InputActionAsset InputActions { get; private set; }
    
    [field: Header("Gameplay Actions")]
    [field: SerializeField] public GameplayInputActions GameplayActions { get; private set; }
    
    [field: Header("UI Actions")]
    [field: SerializeField] public GameplayUIInputActions GameplayUIActions { get; private set; }

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
    public event Action OnUIGameplayResumeInput;

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
            gameplayUIActions.Resume.action.performed += OnResumeInputPerformed;
            
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
            gameplayUIActions.Resume.action.performed -= OnResumeInputPerformed;
            
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
        
        InputActions.Disable();
        
        OCSFXLogger.Log($"[{nameof(InputHandler)}] Disabling current action map ({_currentActionMap.name})", this, _showDebug);
        
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
    }

    [Serializable]
    public abstract class InputActionsCollection
    {
        [field: SerializeField] public InputActionMapRef ActionMapRef { get; private set; }
        public InputActionMap ActionMap => ActionMapRef.Map;
    }
}
