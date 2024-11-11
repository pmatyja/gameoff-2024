using System;
using UnityEngine;

namespace Runtime.Utility
{
    public class DebugLoggerComponent : TriggerOnMonoBehaviourFunction
    {
        public LogType LogType = LogType.Log;
        public string Message;
        
        [Header("Settings")]
        [Tooltip("Toggle to enable/disable logging")]
        public bool Print = true;
        public Color TextColor;

        public void Log()
        {
            if (!Print || string.IsNullOrEmpty(Message)) return;

            var coloredText = $"<color=#{ColorUtility.ToHtmlStringRGB(TextColor)}>{Message}</color>";

            switch (LogType)
            {
                default:
                case LogType.Log:
                    Debug.Log(coloredText, this);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(coloredText, this);
                    break;
                case LogType.Error:
                    Debug.LogError(coloredText, this);
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(coloredText, this);
                    break;
                case LogType.Exception:
                    Debug.LogException(new Exception(coloredText), this);
                    break;
            }
        }
        
        protected override void Trigger() => Log();

        private void Reset()
        {
#if UNITY_EDITOR
            TextColor = UnityEditor.EditorGUIUtility.isProSkin ? Color.white : Color.black;
#endif
        }
    }
}