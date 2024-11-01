namespace Nodes.Actions.Audio
{
    using System;
    using System.Collections;

    [Serializable]
    public class StopAudioLayers : EventProducerNode
    {
        [HideLabel]
        public AudioLayers Layers;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            AudioSources.Stop(this.Layers);
            yield return base.ExcuteAsync(context);
        }
    }
}