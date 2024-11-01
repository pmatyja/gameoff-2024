using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(Animator))]
public class SpriteStateMachine : MonoBehaviour
{
    [SerializeField]
    [Readonly]
    private CharacterController2D characterController;

    [SerializeField]
    [Readonly]
    private Animator animator;

    private void OnEnable()
    {
        this.TryGetComponent(out this.characterController);
        this.TryGetComponent(out this.animator);
    }

    public void Update()
    {
        if (this.characterController.IsGrounded)
        {
            if (this.characterController.IsMoving)
            {
                if (this.characterController.IsCrouching)
                {
                    this.animator.Play("Sneaking");
                }
                else
                {
                    this.animator?.Play("Run");
                }
            }
            else
            {
                if (this.characterController.IsCrouching)
                {
                    this.animator?.Play("CrouchIdle");
                }
                else
                {
                    this.animator?.Play("Idle");
                }
            }
        }
        else
        {
            this.animator?.Play("InAir");
        }
    }
}
