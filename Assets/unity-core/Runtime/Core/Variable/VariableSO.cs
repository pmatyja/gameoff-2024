using UnityEngine;

public class VariableSO<T> : ScriptableObject, IVariable<T>
{
    [SerializeField]
    protected T value;
    public T Value
    {
        get => this.value;
        set => this.Validate(value);
    }

    protected virtual void Validate(T value)
    {
        this.value = value;
    }
}
