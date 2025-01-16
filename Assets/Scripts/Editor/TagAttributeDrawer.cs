using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Runtime.Utility;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class TagAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "Tag attribute can only be used with string", MessageType.Error);
                return;
            }
            
            property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
            
            if (string.IsNullOrEmpty(property.stringValue))
            {
                property.stringValue = "Untagged";
            }
        }
    }
    
    [CustomPropertyDrawer(typeof(TagMaskAttribute))]
    public class TagMaskAttributeDrawer : PropertyDrawer
    {
        private List<string> _tags = new List<string>();
        private List<string> _allTags = new List<string>();
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "TagMask attribute can only be used with string", MessageType.Error);
                return;
            }
            
            // Treat the property like a flags enum field, such that unique strings can be added or removed from a comma-separated string
            _tags = property.stringValue.Split(',').ToList();
            _allTags = UnityEditorInternal.InternalEditorUtility.tags.ToList();
            var newTags = EditorGUI.MaskField(position, label, _tags.Select(tag => _allTags.IndexOf(tag)).Aggregate(0, (mask, index) => mask | 1 << index), _allTags.ToArray());
            
            _tags.Clear();
            _tags.AddRange(_allTags.Where((t, i) => (newTags & 1 << i) != 0));

            property.stringValue = string.Join(",", _tags);
        }
    }
}