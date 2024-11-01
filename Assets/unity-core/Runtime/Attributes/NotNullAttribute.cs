public class NotNullAttribute : BaseAttribute
{
    public readonly string Message;

    public NotNullAttribute(string message = null)
    {
        this.Message = message;
    }
}