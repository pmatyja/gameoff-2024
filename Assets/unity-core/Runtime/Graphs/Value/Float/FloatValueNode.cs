namespace Nodes.Value
{
    using System;
    using UnityEngine;

    [Serializable]
    public class FloatValueNode : ValueNode<float>
    {
        [SerializeReference]
        [TypeInstanceSelector(Label = LabelState.Hidden)]
        public IFloatValueSource Source;

        public override float GetValue()
        {
            return this.InternalGetValue(this.Source);
        }
    }
}