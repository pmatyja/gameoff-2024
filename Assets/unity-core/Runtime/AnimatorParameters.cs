using UnityEngine;

public static class AnimatorParameters
{
    // Boolean

    public static readonly int IsMoving = Animator.StringToHash("IsMoving");
    public static readonly int IsCrouching = Animator.StringToHash("IsCrouching");
    public static readonly int IsWalking = Animator.StringToHash("IsWalking");
    public static readonly int IsRunning = Animator.StringToHash("IsRunning");
    public static readonly int IsJumping = Animator.StringToHash("IsJumping");
    public static readonly int HasJumped = Animator.StringToHash("HasJumped");
    public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    public static readonly int IsFalling = Animator.StringToHash("IsFalling");

    // Float
    public static readonly int LinearVelocity = Animator.StringToHash("LinearVelocity");
    public static readonly int VelocityX = Animator.StringToHash("VelocityX");
    public static readonly int VelocityY = Animator.StringToHash("VelocityY");
}
