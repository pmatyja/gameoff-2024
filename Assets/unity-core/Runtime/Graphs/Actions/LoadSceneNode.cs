using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nodes.Actions.Scene
{
    [Serializable]
    public class LoadSceneNode : ActionNode
    {
        [SceneSelector(Label = LabelState.Hidden)]
        public string Scene;

        [Range(0.0f, 10.0f)]
        public float delay = 0.0f;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            yield return Wait.Seconds(this.delay);

            var state = SceneManager.LoadSceneAsync(this.Scene);
            if (state != null)
            {
                while (state.isDone == false)
                {
                    yield return null;
                }
            }

            yield return base.ExcuteAsync(context);
        }
    }
}