using Runtime.Controllers;
using UnityEngine;

public class PlayerCharacter : OCSFX.Generics.Singleton<PlayerCharacter>
{
    [field: SerializeField] public InputHandler InputHandler { get; private set; }
    [SerializeField] private GameOff2024SimpleWalkController _walkController;

    private void OnEnable()
    {
        InputHandler.OnGameplayMoveInput += OnMoveInput;
    }
    
    private void OnDisable()
    {
        InputHandler.OnGameplayMoveInput -= OnMoveInput;
    }
    
    private void OnMoveInput(Vector2 input)
    {
        _walkController.SetMovementDirection(input);
    }

    private void OnValidate()
    {
        if (!InputHandler)
        {
            InputHandler = InputHandler.Get();
        }
        
        if (!_walkController)
        {
            _walkController = GetComponent<GameOff2024SimpleWalkController>();
        }
    }
}
