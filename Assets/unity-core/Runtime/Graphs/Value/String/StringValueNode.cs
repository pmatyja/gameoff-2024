namespace Nodes.Value
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Serializable]
    public class StringValueNode : ValueNode<string>
    {
        [SerializeReference]
        [TypeInstanceSelector(Label = LabelState.Hidden)]
        public IStringValueSource Source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string GetValue()
        {
            return this.InternalGetValue(this.Source);
        }
    }
}