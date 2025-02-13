using UnityEditor;
using UnityEngine;

namespace OCSFX.EZFMODEditor.CustomEditors
{
        public class EZFMODParameterValueWindow : EditorWindow
        {
            public string inputName;
            public System.Action<string> onNameEntered;

            private void OnEnable()
            {
                minSize = new Vector2(500, 100);
                maxSize = new Vector2(500, 100);
            }

            private void OnGUI()
            {
                GUILayout.Label("Enter a name:", EditorStyles.boldLabel);
                
                var textAreaStyle = new GUIStyle(EditorStyles.textField)
                {
                    wordWrap = true
                };
                inputName = EditorGUILayout.TextArea(inputName, textAreaStyle, GUILayout.Height(50));

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("OK"))
                {
                    onNameEntered?.Invoke(inputName);
                    Close();
                }

                if (GUILayout.Button("Cancel"))
                {
                    Close();
                }
                GUILayout.EndHorizontal();
            }
        }
    
}