using System;

public interface IBooleanValueSource : IValueSource<bool>
{
}

[Serializable]
public class InlineBooleanValueSource : InlineValueSource<bool>, IBooleanValueSource
{
}

[Serializable]
public class MemberBooleanValueSource : MemberValueSource<bool>, IBooleanValueSource
{
}

[Serializable]
public class MethodBooleanValueSource : MethodValueSource<bool>, IBooleanValueSource
{
}

[Serializable]
public class VariableBooleanValueSource : VariableValueSource<bool>, IBooleanValueSource
{
}