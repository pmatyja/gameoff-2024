using OCSFX.EZFMOD.Types;
using UnityEditor;
using UnityEngine;

namespace OCSFX.EZFMODEditor.CustomEditors
{
    [CustomEditor(typeof(EZFMODParameterValue), true)]
    public class EZFMODParameterValueEditor : UnityEditor.Editor
    {
        private bool _cachedGuiState;
        private SerializedProperty _parameter;
        private SerializedProperty _value;

        public override void OnInspectorGUI()
        {
            var parameterValue = (EZFMODParameterValue)serializedObject.targetObject;
            if (!parameterValue)
            {
                return;
            }

            BeginReadOnlyInspector();

            _parameter = serializedObject.FindProperty(nameof(_parameter));
            _parameter.objectReferenceValue = EditorGUILayout.ObjectField("Parameter", _parameter.objectReferenceValue, typeof(EZFMODParameter), false);

            EndReadOnlyInspector();
            if (_parameter.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Parameter is required.", MessageType.Error);
                return;
            }

            var parameter = (EZFMODParameter)_parameter.objectReferenceValue;

            Undo.RecordObject(parameterValue, "Modify ParameterValue");
            
            _value = serializedObject.FindProperty(nameof(_value));
            switch (parameter.Type)
            {
                case ParameterType.Continuous:
                    HandleContinuousParameter(parameter);
                    break;
                case ParameterType.Discrete:
                    HandleDiscreteParameter(parameter);
                    break;
                case ParameterType.Labeled:
                    HandleLabeledParameter(parameter);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void BeginReadOnlyInspector()
        {
            _cachedGuiState = GUI.enabled;
            GUI.enabled = false;
        }

        private void EndReadOnlyInspector()
        {
            GUI.enabled = _cachedGuiState;
        }

        private void HandleContinuousParameter(EZFMODParameter parameter)
        {
            Undo.RecordObject(serializedObject.targetObject, "Edit Continuous Parameter Value");
            _value.floatValue = EditorGUILayout.Slider("User-Defined Value", _value.floatValue, (float)parameter.Min, (float)parameter.Max);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Reset to Default"))
            {
                _value.floatValue = (float)parameter.Default;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Separator();

            if (GUILayout.Button("Rename"))
            {
                Debug.Log("Rename");
                EZFMODParameterContextMenu.RenameParamValueSubobject();
            }

            var cachedGuiColor = GUI.color;

            GUI.color = Color.red;
            if (GUILayout.Button("Delete"))
            {
                Debug.Log("Delete");
                EZFMODParameterContextMenu.DeleteParamValueSubobject();
            }
            GUI.color = cachedGuiColor;

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        private void HandleDiscreteParameter(EZFMODParameter parameter)
        {
            Undo.RecordObject(serializedObject.targetObject, "Edit Discrete Parameter Value");
            BeginReadOnlyInspector();
            _value.floatValue = EditorGUILayout.IntSlider("Discrete Value", (int)_value.floatValue, (int)parameter.Min, (int)parameter.Max);
            EndReadOnlyInspector();
        }

        private void HandleLabeledParameter(EZFMODParameter parameter)
        {
            Undo.RecordObject(serializedObject.targetObject, "Edit Labeled Parameter Value");
            BeginReadOnlyInspector();
            var index = EditorGUILayout.Popup("Labeled Value", (int)_value.floatValue, parameter.Labels);
            _value.floatValue = index;
            EndReadOnlyInspector();
        }
    }
}