using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
[DisplayName("Member Value")]
public class MemberValueSource<T> : IValueSource<T>
{
    [HideLabel]
    public UnityEngine.Object Source;

    [SerializeReference]
    [TypeMemberSelector(nameof(Source), nameof(MemberType), Label = LabelState.Hidden)]
    public string Member;

    private static Type MemberType => typeof(T);

    private ReferenceInfo result;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetValue()
    {
        return UnityMember.TryGetValue<T>(this.Source, this.Member, ref this.result);
    }
}
