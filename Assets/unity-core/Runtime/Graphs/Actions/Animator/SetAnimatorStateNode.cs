using System;
using System.Collections;
using UnityEngine;

namespace Nodes.Actions.Animations
{
    [Serializable]
    public class SetAnimatorStateNode : ActionNode
    {
        [HideLabel]
        public Animator Animator;

        [AnimationSelector(nameof(Animator), Label = LabelState.Hidden)]
        public string State;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            if (string.IsNullOrWhiteSpace(this.State) == false)
            {
                this.Animator.Play(this.State);
            }

            yield return base.ExcuteAsync(context);
        }
    }
}
