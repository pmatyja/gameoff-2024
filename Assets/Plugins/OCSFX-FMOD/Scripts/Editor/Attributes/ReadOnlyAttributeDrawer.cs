using OCSFX.EZFMOD.Attributes;
using UnityEditor;
using UnityEngine;

namespace OCSFX.EZFMODEditor.Attributes
{ 
    /** <summary>
    * Disables the ability to edit a field in the inspector.
    * </summary>
    */
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);   
            EditorGUI.EndDisabledGroup();
        }
    }   
}
