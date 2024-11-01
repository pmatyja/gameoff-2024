using System;
using System.ComponentModel;

[Serializable]
[DisplayName("Object")]
public class ObjectValueSource<T> : IValueSource<T> where T : UnityEngine.Object
{
    [ObjectPicker(nameof(ObjectType), Label = LabelState.Hidden)]
    public T Value;

    private static Type ObjectType => typeof(T);

    public T GetValue()
    {
        return this.Value ?? default(T);
    }
}