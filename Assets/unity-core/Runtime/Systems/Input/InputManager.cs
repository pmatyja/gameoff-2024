using System.Collections.Concurrent;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class InputManager : Singleton<InputManager>
{
    [SerializeField]
    [NotNull("File 'Shared/InputAction.inputactions' not found")]
    private InputActionAsset input;

    private static ConcurrentDictionary<string, InputState> States = new();

    private class InputState
    {
        public bool Consumed;
        public UnityEngine.InputSystem.InputAction.CallbackContext Context;
    }

    public static Vector2 Linear(string action)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            return Vector2.zero;
        }

        if (States.TryGetValue(action, out var state))
        {
            return state.Context.ReadValue<Vector2>();
        }

        return Vector2.zero;
    }

    public static bool Pressed(string action)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            return false;
        }

        if (States.TryGetValue(action, out var state))
        {
            return state.Context.ReadValueAsButton();
        }

        return false;
    }

    public static bool Released(string action)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            return false;
        }

        if (States.TryGetValue(action, out var state))
        {
            if (state.Context.ReadValueAsButton())
            {
                if (state.Consumed == false)
                {
                    state.Consumed = true;
                    return true;
                }
            }
        }

        return false;
    }

    public void SetInput(string actionMap)
    {
        States.Clear();

        foreach (var action in this.input)
        {
            action.Disable();
            action.Reset();
        }

        var map = this.input.FindActionMap(actionMap);
        if (map == null)
        {
            Debug.LogWarning($"No '{actionMap}' action map found");
            return;
        }

        map.Enable();
    }

    private void Start()
    {
        this.SetInput(GameManager.Mode.ToString());
    }

    private void OnEnable()
    {
        States.Clear();
        
        EventBus.AddListener<GameModeEventParameters>(this.OnModeChange);

        foreach (var action in this.input)
        {
            action.started      += this.OnStarted;
            action.performed    += this.OnPerformed;
            action.canceled     += this.OnCanceled;
        }
    }

    private void OnDisable()
    {
        EventBus.RemoveListener<GameModeEventParameters>(this.OnModeChange);

        foreach (var action in this.input)
        {
            action.started      -= this.OnStarted;
            action.performed    -= this.OnPerformed;
            action.canceled     -= this.OnCanceled;
        }
    }

    private void OnValidate()
    {
        this.input = EditorOnly.LoadAsset<InputActionAsset>("Assets/Resources/Shared/InputAction.inputactions");
    }

    private void OnModeChange(object sender, GameModeEventParameters parameters)
    {
        this.SetInput(parameters.Mode.ToString());
    }

    private void OnStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        States.AddOrUpdate($"{context.action.actionMap.asset.name}/{context.action.actionMap.name}/{context.action.name}", 
            (key) =>
            {
                return new InputState
                {
                    Consumed = false,
                    Context = context
                };
            }, 
            (key, old) =>
            {
                old.Consumed = false;
                old.Context = context;

                return old;
            });
    }

    private void OnPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        States.AddOrUpdate($"{context.action.actionMap.asset.name}/{context.action.actionMap.name}/{context.action.name}",
            (key) =>
            {
                return new InputState
                {
                    Consumed = false,
                    Context = context
                };
            },
            (key, old) =>
            {
                old.Context = context;
                return old;
            });
    }

    private void OnCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        States.AddOrUpdate($"{context.action.actionMap.asset.name}/{context.action.actionMap.name}/{context.action.name}",
            (key) =>
            {
                return new InputState
                {
                    Consumed = false,
                    Context = context
                };
            },
            (key, old) =>
            {
                old.Context = context;
                return old;
            });
    }
}
