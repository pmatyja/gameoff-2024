using System;
using System.Linq;
using Runtime.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Editor
{
    [CustomPropertyDrawer(typeof(InputActionMapRef))]
    public class InputActionMapRefDrawer : PropertyDrawer
    {
        private SerializedProperty _inputActionAsset;
        private SerializedProperty _name;
        private SerializedProperty _map;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _inputActionAsset = property.FindPropertyRelative(nameof(_inputActionAsset));
            _name = property.FindPropertyRelative(nameof(_name));
            _map = property.FindPropertyRelative(nameof(_map));

            if (_name == null)
            {
                Debug.LogError(
                    $"{nameof(SerializedProperty)} is null. " +
                    $"Ensure that the field name '{nameof(_name)}' exists in the {nameof(InputActionMapRef)} class.");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var fieldWidth = position.width / 2;

            var inputActionAssetRect = new Rect(position.x, position.y, fieldWidth, position.height);
            var nameRect = new Rect(position.x + fieldWidth, position.y, fieldWidth, position.height);

            EditorGUI.PropertyField(inputActionAssetRect, _inputActionAsset, GUIContent.none);

            var inputActionAsset = _inputActionAsset.objectReferenceValue as InputActionAsset;

            if (inputActionAsset)
            {
                var mapNames = inputActionAsset.actionMaps.Select(map => map.name).ToArray();
                var selectedIndex = Mathf.Max(0, Array.IndexOf(mapNames, _name.stringValue));
                selectedIndex = EditorGUI.Popup(nameRect, selectedIndex, mapNames);
                _name.stringValue = mapNames[selectedIndex];
                _map.boxedValue = inputActionAsset.FindActionMap(_name.stringValue);
            }
            else
            {
                EditorGUI.PropertyField(nameRect, _name, GUIContent.none);
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
            
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}