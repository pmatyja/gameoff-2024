using System;
using UnityEngine;

namespace Runtime.Utility
{
    public class DebugLoggerComponent : MonoBehaviour
    {
        [SerializeField] private LogType _logType = LogType.Log;
        [SerializeField] private string _message;
        
        [Header("Settings")]
        [Tooltip("Toggle to enable/disable logging")]
        [SerializeField] private bool _print = true;
        [SerializeField] private Color _textColor;

        public void Log()
        {
            if (!_print) return;
            if (string.IsNullOrEmpty(_message)) return;
            
            var coloredText = $"<color=#{ColorUtility.ToHtmlStringRGB(_textColor)}>{_message}</color>";
            
            switch (_logType)
            {
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Reset()
        {
#if UNITY_EDITOR
            _textColor = UnityEditor.EditorGUIUtility.isProSkin ? Color.white : Color.black;
#endif
        }
    }
}