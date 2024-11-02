using UnityEngine;
using Object = UnityEngine.Object;

namespace OCSFX.Utility.Debug
{
    public static class OCSFXLogger
    {
        public static void Log(object message, Object context, bool condition = true)
        {
            if (!condition) return;
            LogInternal(LogType.Log, message, context);
        }
        
        public static void Log(object message, bool condition)
        {
            if (!condition) return;
            LogInternal(LogType.Log, message);
        }
        
        public static void Log(object message) => LogInternal(LogType.Log, message);
        
        public static void LogWarning(object message, Object context, bool condition = true)
        {
            if (!condition) return;
            LogInternal(LogType.Warning, message, context);
        }
        
        public static void LogWarning(object message, bool condition)
        {
            if (!condition) return;
            LogInternal(LogType.Warning, message);
        }
        
        public static void LogWarning(object message) => LogInternal(LogType.Warning, message);
        
        public static void LogError(object message, Object context, bool condition = true)
        {
            if (!condition) return;
            LogInternal(LogType.Error, message, context);
        }
        
        public static void LogError(object message, bool condition)
        {
            if (!condition) return;
            LogInternal(LogType.Error, message);
        }
        
        public static void LogError(object message) => LogInternal(LogType.Error, message);
        
        private static void LogInternal(LogType logType, object message, Object context = null)
        {
            if (context)
                UnityEngine.Debug.unityLogger.Log(logType, message, context);
            else
                UnityEngine.Debug.unityLogger.Log(logType, message);
        }
    }
}
