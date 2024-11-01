using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

[DisplayName("Method Result")]
public abstract class MethodValueSource<T> : IValueSource<T>
{
    [HideLabel]
    public UnityEngine.Object Source;

    [SerializeReference]
    [TypeMethodSelector(nameof(Source), nameof(ReturnType), Label = LabelState.Hidden)]
    public string Method;

    private static Type ReturnType => typeof(T);

    private MethodInfo result;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetValue()
    {
        return UnityMethod.TryInvoke<T>(this.Source, this.Method, ref this.result);
    }
}
