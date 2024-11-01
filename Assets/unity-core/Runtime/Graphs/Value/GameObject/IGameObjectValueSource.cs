using System;

public interface IGameObjectValueSource : IValueSource<UnityEngine.GameObject>
{
}

[Serializable]
public class InlineGameObjectValueSource : InlineValueSource<UnityEngine.GameObject>, IGameObjectValueSource
{
}

[Serializable]
public class MemberGameObjectValueSource : MemberValueSource<UnityEngine.GameObject>, IGameObjectValueSource
{
}

[Serializable]
public class MethodGameObjectValueSource : MethodValueSource<UnityEngine.GameObject>, IGameObjectValueSource
{
}

[Serializable]
public class ObjectGameObjectValueSource : ObjectValueSource<UnityEngine.GameObject>, IGameObjectValueSource
{
}