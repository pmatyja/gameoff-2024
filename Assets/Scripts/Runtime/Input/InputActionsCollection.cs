using System;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Input
{
    [Serializable]
    public abstract class InputActionsCollection
    {
        [field: SerializeField] public InputActionMapRef ActionMapRef { get; private set; }
        public InputActionMap ActionMap => ActionMapRef.Map;
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
}