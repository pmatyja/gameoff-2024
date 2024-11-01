using System;

public interface IStringValueSource : IValueSource<string>
{
}

[Serializable]
public class InlineStringValueSource : InlineValueSource<string>, IStringValueSource
{
}

[Serializable]
public class MemberStringValueSource : MemberValueSource<string>, IStringValueSource
{
}

[Serializable]
public class MethodStringValueSource : MethodValueSource<string>, IStringValueSource
{
}

[Serializable]
public class VariableStringValueSource : VariableValueSource<string>, IStringValueSource
{
}