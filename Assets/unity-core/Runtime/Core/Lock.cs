public static class Locked
{
    public static T Exchange<T>(in object locker, ref T value, T newValue = default)
    {
        var oldValue = default(T);

        lock (locker)
        {
            oldValue = value;
            value = newValue;
        }

        return oldValue;
    }
}