using System;

[Serializable]
public class CoroutineResult<T>
{
    public T Value { get; private set; }

    public void SetResult(T value)
    {
        this.Value = value;
    }
}
