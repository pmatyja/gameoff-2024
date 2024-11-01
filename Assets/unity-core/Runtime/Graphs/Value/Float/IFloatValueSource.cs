using System;

public interface IFloatValueSource : IValueSource<float>
{
}

[Serializable]
public class InlineFloatValueSource : InlineValueSource<float>, IFloatValueSource
{
}

[Serializable]
public class MemberFloatValueSource : MemberValueSource<float>, IFloatValueSource
{
}

[Serializable]
public class MethodFloatValueSource : MethodValueSource<float>, IFloatValueSource
{
}

[Serializable]
public class VariableFloatValueSource : VariableValueSource<float>, IFloatValueSource
{
}