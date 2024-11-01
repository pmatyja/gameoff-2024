namespace Nodes.Value
{
    using System;

    [Serializable]
    public class BooleanGateValueNode : ValueNode<bool>
    {
        public GateType Type;

        [Parameter]
        public ValueNode<bool> A;

        [Parameter]
        public ValueNode<bool> B;

        public enum GateType
        {
            AND,
            OR,
            XOR
        }

        public override bool GetValue()
        {
            if (this.Type == GateType.AND)
            {
                if (this.A == null || this.B == null)
                {
                    return false;
                }

                return this.A.GetValue() && this.B.GetValue();
            }
            
            if (this.Type == GateType.OR)
            {
                if (this.A != null && this.A.GetValue())
                {
                    return true;
                }

                if (this.B != null && this.B.GetValue())
                {
                    return true;
                }

                return false;
            }

            if (this.Type == GateType.XOR)
            {
                if (this.A == null)
                {
                    return this.B?.GetValue() ?? false;
                }

                if (this.B == null)
                {
                    return this.A?.GetValue() ?? false;
                }

                return this.A.GetValue() != this.B.GetValue();
            }

            throw new NotImplementedException($"'{this.Type}' is not implemented");
        }
    }
}