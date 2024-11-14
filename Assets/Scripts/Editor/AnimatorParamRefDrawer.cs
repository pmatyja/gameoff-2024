using System;
using System.Collections.Generic;
using Runtime.Utility;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AnimatorParamRef))]
    public class AnimatorParamRefDrawer : PropertyDrawer
    {
        private SerializedProperty _animator;
        private SerializedProperty _name;
        private SerializedProperty _type;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _animator = property.FindPropertyRelative(nameof(_animator));
            _name = property.FindPropertyRelative(nameof(_name));
            _type = property.FindPropertyRelative(nameof(_type));

            if (_name == null || _type == null)
            {
                Debug.LogError(
                    $"{nameof(SerializedProperty)} is null. " +
                    $"Ensure that the field names '{nameof(_name)}' and '{nameof(_type)}' exist in the {nameof(AnimatorParamRef)} class.");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var fieldWidth = position.width / 3;

            var animatorRect = new Rect(position.x, position.y, fieldWidth, position.height);
            var nameRect = new Rect(position.x + fieldWidth, position.y, fieldWidth, position.height);
            var typeRect = new Rect(position.x + fieldWidth * 2, position.y, fieldWidth, position.height);

            EditorGUI.PropertyField(animatorRect, _animator, GUIContent.none);

            if (_animator.objectReferenceValue is Animator animator)
            {
                var parameterInfo = GetAnimatorParameterInfo(animator);
                var parameterNames = new List<string>(parameterInfo.Keys);
                var selectedIndex = Mathf.Max(0, parameterNames.IndexOf(_name.stringValue));
                selectedIndex = EditorGUI.Popup(nameRect, selectedIndex, parameterNames.ToArray());
                _name.stringValue = parameterNames[selectedIndex];
                _type.intValue = (int)parameterInfo[_name.stringValue];
            }
            else
            {
                EditorGUI.PropertyField(nameRect, _name, GUIContent.none);
            }

            EditorGUI.PropertyField(typeRect, _type, GUIContent.none);

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private Dictionary<string, AnimatorControllerParameterType> GetAnimatorParameterInfo(Animator animator)
        {
            var parameters = animator.parameters;
            var parameterInfo = new Dictionary<string, AnimatorControllerParameterType>();
            foreach (var parameter in parameters)
            {
                parameterInfo[parameter.name] = parameter.type;
            }
            return parameterInfo;
        }
    }
}