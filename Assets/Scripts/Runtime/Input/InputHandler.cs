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
    
    [SerializeField] private bool _showDebug;

    public event Action<Vector2> OnMoveInput;
    public event Action<bool> OnDragCameraInput;

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
            Get().InputActions.FindAction("Move").performed += OnMoveInputPerformed;
            Get().InputActions.FindAction("Move").canceled += OnMoveInputPerformed;
            
            Get().InputActions.FindAction("DragCamera").performed += OnDragCameraInputPerformed;
            Get().InputActions.FindAction("DragCamera").canceled += OnDragCameraInputPerformed;
            
            Application.quitting += Deinitialize;
        }
        else
        {
            Get().InputActions.FindAction("Move").performed -= OnMoveInputPerformed;
            Get().InputActions.FindAction("Move").canceled -= OnMoveInputPerformed;
            
            Get().InputActions.FindAction("DragCamera").performed -= OnDragCameraInputPerformed;
            Get().InputActions.FindAction("DragCamera").canceled -= OnDragCameraInputPerformed;
            
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
}
