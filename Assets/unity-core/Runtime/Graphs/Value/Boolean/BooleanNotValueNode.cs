namespace Nodes.Value
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class BooleanNotValueNode : ValueNode<bool>
    {
        public override float Width => 200;

        [Parameter]
        public ValueNode<bool> In;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool GetValue()
        {
            return !this.In?.GetValue() ?? false;
        }
    }
}