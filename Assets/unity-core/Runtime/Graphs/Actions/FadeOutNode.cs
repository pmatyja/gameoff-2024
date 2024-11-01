using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nodes.Actions.Scene
{
    [Serializable]
    public class FadeOutNode : ActionNode
    {
        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            EventBus.Raise(this, new FadeOutEventParameters());
            yield return base.ExcuteAsync(context);
        }
    }
}