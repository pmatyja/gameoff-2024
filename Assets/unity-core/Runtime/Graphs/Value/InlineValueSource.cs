using System.ComponentModel;

[DisplayName("Inline Value")]
public abstract class InlineValueSource<T> : IValueSource<T>
{
    [HideLabel]
    public T Value;

    public T GetValue()
    {
        return this.Value;
    }
}
