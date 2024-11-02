using OCSFX.Utility.Attributes;
using UnityEditor;
using UnityEngine;

namespace OCSFXEditor.Attributes
{
    [CustomPropertyDrawer(typeof(ExpandableAttribute))]
    public class ExpandableAttributeDrawer: PropertyDrawer
    {
        private UnityEditor.Editor _editor;
        private bool _expandedState;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);

            if (!property.objectReferenceValue) return;

            var expansionLabel = property.isExpanded ? "[ - ]" : "[ + ]";
            // var expansionLabel = property.isExpanded ? "[Close]" : $"[Inspect {label}]";

            var contentColor = GUI.contentColor;
            var foldoutCollapsedColor = new Color(GUI.contentColor.r * 0.8f, GUI.contentColor.g, GUI.contentColor.b, 0.6f);
            var foldoutExpandedColor = new Color(GUI.contentColor.r, GUI.contentColor.g * .8f, GUI.contentColor.b *.8f, 0.6f);
            GUI.contentColor = property.isExpanded ? foldoutExpandedColor  : foldoutCollapsedColor;

            EditorGUI.indentLevel++;
            DrawFoldout(property, expansionLabel, contentColor);
            EditorGUI.indentLevel--;
            
            GUI.contentColor = contentColor;
        }

        private void DrawFoldout(SerializedProperty property, string foldoutLabel, Color contentColor)
        {
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, foldoutLabel, true);
            
            if (property.isExpanded)
            {
                GUI.contentColor = contentColor;
                DrawPropertyEditorInspector(property);
            }
        }

        private void DrawButtonFoldout(SerializedProperty property, string buttonLabel, Color contentColor)
        {
            if (GUILayout.Button(buttonLabel))
            {
                _expandedState = !_expandedState;
            }

            property.isExpanded = _expandedState;
            
            if (property.isExpanded)
            {
                GUI.contentColor = contentColor;
                DrawPropertyEditorInspector(property);
            }
        }

        private void DrawPropertyEditorInspector(SerializedProperty property)
        {
            EditorGUI.indentLevel++;

            var defaultBgColor = GUI.backgroundColor;
            var bgColor = new Color(GUI.contentColor.r * 0.98f, GUI.contentColor.g, GUI.contentColor.b, 1f);
            GUI.backgroundColor = bgColor;
            
            var rect = EditorGUILayout.BeginVertical(GUI.skin.box);

            if (!_editor)
                UnityEditor.Editor.CreateCachedEditor(property.objectReferenceValue, null, ref _editor);

            _editor.OnInspectorGUI();

            EditorGUILayout.EndVertical();

            var outlineColor = new Color(GUI.contentColor.r * 0.35f, GUI.contentColor.g *.5f, GUI.contentColor.b *.5f, 1f);
            DrawOutlineBox(rect, outlineColor, 1);

            EditorGUI.indentLevel--;
            GUI.backgroundColor = defaultBgColor;
        }
        
        private void DrawOutlineBox(Rect rect, Color color, int thickness)
        {
            var halfThickness = (int)(thickness * 0.5f);
            
            var topLine = new Rect(rect.x, rect.y, rect.width - halfThickness, thickness);
            var bottomLine = new Rect(rect.x, rect.yMax - halfThickness, rect.width - halfThickness, thickness);
            var leftLine = new Rect(rect.x, rect.y, thickness, rect.height - halfThickness);
            var rightLine = new Rect(rect.xMax - halfThickness, rect.y, thickness, rect.height + halfThickness);
            
            EditorGUI.DrawRect(topLine, color);
            EditorGUI.DrawRect(leftLine, color);
            EditorGUI.DrawRect(bottomLine, color);
            EditorGUI.DrawRect(rightLine, color);
        }
    }
}