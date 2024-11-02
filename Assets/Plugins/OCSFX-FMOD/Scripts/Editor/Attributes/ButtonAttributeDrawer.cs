using OCSFX.Utility.Attributes;
using UnityEditor;
using UnityEngine;

namespace OCSFXEditor.Attributes
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonDrawer : PropertyDrawer
    {
        private SerializedProperty _cachedProperty;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_cachedProperty == null) _cachedProperty = property;
            if (_cachedProperty == null) return;

            if (_cachedProperty.serializedObject == null) return;
            if (_cachedProperty.serializedObject.targetObject == null) return;
            
            var target = _cachedProperty.serializedObject.targetObject;
            if (target == null) return;
            
            var type = target.GetType();
            var methodName = (attribute as ButtonAttribute)?.MethodName;
            if (methodName == null) return;
            
            var method = type.GetMethod(methodName);
            if (method == null)
            {
                GUI.Label(position, "Method could not be found. Is it public?");
                return;
            }
            if (method.GetParameters().Length > 0)
            {
                GUI.Label(position, "Method cannot have parameters.");
                return;
            }
            
            if (GUI.Button(position, method.Name))
            {
                method.Invoke(target, null);
            }
        }
    }

}