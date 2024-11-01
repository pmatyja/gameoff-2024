using System;

public interface IIntValueSource : IValueSource<int>
{
}

[Serializable]
public class InlineIntValueSource : InlineValueSource<int>, IIntValueSource
{
}

[Serializable]
public class MemberIntValueSource : MemberValueSource<int>, IIntValueSource
{
}

[Serializable]
public class MethodIntValueSource : MethodValueSource<int>, IIntValueSource
{
}

[Serializable]
public class VariableIntValueSource : VariableValueSource<int>, IIntValueSource
{
}