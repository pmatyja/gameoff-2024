using System;
using UnityEngine;

public sealed class PlayerController2D : CharacterController2D
{
    [Header("Input")]

    [SerializeField]
    [InputActionMap]
    private string inputMove;

    [SerializeField]
    [InputActionMap]
    private string inputJump;

    [SerializeField]
    [InputActionMap]
    private string inputRun;

    [SerializeField]
    [InputActionMap]
    private string inputCrouch;

    [SerializeField]
    [InputActionMap]
    private string inputAction;

    private void Update()
    {
        this.Crouch(InputManager.Pressed(this.inputCrouch));
        this.Run(InputManager.Pressed(this.inputRun));
        this.Jump(InputManager.Pressed(this.inputJump));
        this.Move(new Vector2(InputManager.Linear(this.inputMove).x, 0.0f));
    }
}
