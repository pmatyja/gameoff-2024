using Runtime.Controllers;
using Runtime.Input;
using UnityEngine;

public class PlayerCharacter : OCSFX.Generics.Singleton<PlayerCharacter>
{
    [SerializeField] private GameOff2024SimpleWalkController _walkController;

    public IInputHandler InputHandler { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        SetInputHandler(UserInputHandler.Get());
    }
    
    public void SetInputHandler(IInputHandler inputHandler)
    {
        if (InputHandler == inputHandler) return;

        if (InputHandler != null)
        {
            SetInputHandlerBindings(InputHandler, false);    
        }
        
        SetInputHandlerBindings(inputHandler, true);
        
        InputHandler = inputHandler;
    }
    
    private void SetInputHandlerBindings(IInputHandler inputHandler, bool bind)
    {
        if (bind)
        {
            inputHandler.OnGameplayMoveInput += OnMoveInput;
            inputHandler.OnGameplayCameraZoomInput += OnCameraZoomInput;
            inputHandler.OnGameplayInteractInput += OnInteractInput;
        }
        else
        {
            inputHandler.OnGameplayMoveInput -= OnMoveInput;
            inputHandler.OnGameplayCameraZoomInput -= OnCameraZoomInput;
            inputHandler.OnGameplayInteractInput -= OnInteractInput;
        }
    }

    private void OnMoveInput(Vector2 input)
    {
        _walkController.SetMovementDirection(input);
    }

    private void OnCameraZoomInput(float zoom)
    {
    }
    
    private void OnInteractInput()
    {
    }

    private void OnValidate()
    {
        if (!_walkController)
        {
            _walkController = GetComponent<GameOff2024SimpleWalkController>();
        }
    }
}
