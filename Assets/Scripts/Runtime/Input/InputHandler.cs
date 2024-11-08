using System;
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
    
    [field: Header("Gameplay Actions")]
    [field: SerializeField] public InputActionReference MoveActionRef { get; private set; }
    [field: SerializeField] public InputActionReference CameraDragActionRef { get; private set; }
    [field: SerializeField] public InputActionReference CameraZoomActionRef { get; private set; }
    
    [Header("Debug")]
    [SerializeField] private bool _showDebug;

    public event Action<Vector2> OnMoveInput;
    public event Action<bool> OnDragCameraInput;
    public event Action<float> OnZoomInput; 

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
            Get().MoveActionRef.action.performed += OnMoveInputPerformed;
            Get().MoveActionRef.action.canceled += OnMoveInputPerformed;
            
            Get().CameraDragActionRef.action.performed += OnDragCameraInputPerformed;
            Get().CameraDragActionRef.action.canceled += OnDragCameraInputPerformed;
            
            Get().CameraZoomActionRef.action.performed += OnZoomInputPerformed;
            
            Application.quitting += Deinitialize;
        }
        else
        {
            Get().MoveActionRef.action.performed -= OnMoveInputPerformed;
            Get().MoveActionRef.action.canceled -= OnMoveInputPerformed;
            
            Get().CameraDragActionRef.action.performed -= OnDragCameraInputPerformed;
            Get().CameraDragActionRef.action.canceled -= OnDragCameraInputPerformed;
            
            Get().CameraZoomActionRef.action.performed -= OnZoomInputPerformed;
            
            Application.quitting -= Deinitialize;
        }
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
        
        var value = context.ReadValue<float>();
        if (Mathf.Approximately(value, 0)) return;
        
        OCSFXLogger.Log($"Zoom input performed ({value})", Get(), Get()._showDebug);
        
        Get().OnZoomInput?.Invoke(value);
    }
}
