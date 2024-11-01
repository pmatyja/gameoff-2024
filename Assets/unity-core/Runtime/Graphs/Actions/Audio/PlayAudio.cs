namespace Nodes.Actions.Audio
{
    using System;
    using System.Collections;
    using UnityEngine;

    [Serializable]
    public class PlayAudio : EventProducerNode
    {
        [HideLabel]
        [Expandable]
        public AudioResourceSO Resource;

        public GameObject Ancor;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            this.Resource?.Play(this.Ancor);
            yield return base.ExcuteAsync(context);
        }
    }
}