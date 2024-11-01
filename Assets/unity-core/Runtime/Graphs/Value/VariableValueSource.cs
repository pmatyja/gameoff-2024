using System;
using System.ComponentModel;

[Serializable]
[DisplayName("Variable")]
public class VariableValueSource<T> : IValueSource<T>
{
    [ObjectPicker(nameof(ObjectType), Label = LabelState.Hidden)]
    public VariableSO<T> Value;

    private static Type ObjectType => typeof(VariableSO<T>);

    public T GetValue()
    {
        if (this.Value == null)
        {
            return default(T);
        }

        return this.Value.Value ?? default(T);
    }
}