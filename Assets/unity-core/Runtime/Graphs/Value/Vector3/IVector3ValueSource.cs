using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public interface IVector3ValueSource : IValueSource<UnityEngine.Vector3>
{
}

[Serializable]
public class InlineVector3ValueSource : InlineValueSource<UnityEngine.Vector3>, IVector3ValueSource
{
}

[Serializable]
public class MemberVector3ValueSource : MemberValueSource<UnityEngine.Vector3>, IVector3ValueSource
{
}

[Serializable]
public class MethodVector3ValueSource : MethodValueSource<UnityEngine.Vector3>, IVector3ValueSource
{
}

[Serializable]
[DisplayName("GameObject Position")]
public class ObjectVector3ValueSource : IVector3ValueSource
{
    [ObjectPicker(typeof(GameObject), Label = LabelState.Hidden)]
    public GameObject Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 GetValue()
    {
        return this.Value?.transform?.position ?? Vector3.zero;
    }
}