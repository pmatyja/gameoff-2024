using UnityEngine;

public class Logger : Singleton<Logger>
{
    #if DEBUG
        public LoggingLevel LogLevel = LoggingLevel.Developer;
    #else
        public LoggingLevel LogLevel = LoggingLevel.Info;
    #endif

    private static LoggingLevel level => Logger.Instance?.LogLevel ?? LoggingLevel.Info;

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Developer(string message)
    {
        if (level <= LoggingLevel.Developer)
        {
            Debug.Log(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Info(string message)
    {
        if (level <= LoggingLevel.Info)
        {
            Debug.Log(message);
        }
    }

    public static void Warning(string message)
    {
        if (level <= LoggingLevel.Warning)
        {
            Debug.LogWarning(message);
        }
    }

    public static void Error(string message)
    {
        if (level <= LoggingLevel.Error)
        {
            Debug.LogError(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Developer(LoggingLevel level, string message)
    {
        if (level <= LoggingLevel.Developer)
        {
            Debug.Log(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Info(LoggingLevel level, string message)
    {
        if (level <= LoggingLevel.Info)
        {
            Debug.Log(message);
        }
    }

    public static void Warning(LoggingLevel level, string message)
    {
        if (level <= LoggingLevel.Warning)
        {
            Debug.LogWarning(message);
        }
    }

    public static void Error(LoggingLevel level, string message)
    {
        if (level <= LoggingLevel.Error)
        {
            Debug.LogError(message);
        }
    }
}