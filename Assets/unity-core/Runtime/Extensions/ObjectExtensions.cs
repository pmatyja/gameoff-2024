using System;
using System.ComponentModel;

public static class ObjectExtensions
{
    public static bool TryConvertTo(this object value, Type type, out object result)
    {
        if (value == null)
        {
            result = null;
            return true;
        }

        if (type.IsPrimitive)
        {
            var converter = TypeDescriptor.GetConverter(type);
            if (converter != null)
            {
                var stringValue = value?.ToString();

                if (converter.CanConvertFrom(typeof(string)))
                {
                    try
                    {
                        result = converter.ConvertFromString(stringValue ?? string.Empty);
                        return true;
                    }
                    catch (Exception)
                    {
                        result = null;
                        return false;
                    }
                }
            }
        }

        if (type.IsEnum)
        {
            if (value is string str)
            {
                if (Enum.TryParse(type, str, out var parsedEnum))
                {
                    result = parsedEnum;
                    return true;
                }

                result = null;
                return false;
            }
        }

        result = value;
        return true;
    }
}
