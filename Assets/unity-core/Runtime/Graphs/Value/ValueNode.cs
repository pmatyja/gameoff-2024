namespace Nodes.Value
{
    using System.Runtime.CompilerServices;

    public abstract class ValueNode<T> : IValueNode
    {
        public virtual float Width { get; } = 288.0f;
        public virtual string BackgroundColor { get; } = "#2E5739";


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract T GetValue();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T InternalGetValue(IValueSource<T> source)
        {
            if (source == null)
            {
                return default;
            }

            return source.GetValue();
        }
    }
}