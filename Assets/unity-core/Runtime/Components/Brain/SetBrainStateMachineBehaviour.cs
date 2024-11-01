using UnityEngine;

public class SetBrainStateMachineBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private BrainActivitySO Activity;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.TryGetComponent<BrainBehvaiour>(out var brain))
        {
            brain.SetActivity(this.Activity);
        }
    }
}
