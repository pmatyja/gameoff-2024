using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ExpandableAttribute))]
public class ExpandableAttributeDrawer : BasePropertyDrawer
{
    private UnityEditor.Editor editor;
    private static readonly Color border = new Color(0.1f, 0.1f, 0.1f);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        this.FoldableContent(position, property, label, rect =>
        {
            if (property.objectReferenceValue == null)
            {
                return;
            }

            EditorGUI.indentLevel++;

            var content = EditorGUILayout.BeginVertical(GUI.skin.box);

            if (this.editor == null)
            {
                UnityEditor.Editor.CreateCachedEditor(property.objectReferenceValue, null, ref this.editor);
            }

            EditorGUI.DrawRect(new Rect(content.x, content.y, content.width, 1), border);
            EditorGUI.DrawRect(new Rect(content.x, content.y, 1, content.height), border);
            EditorGUI.DrawRect(new Rect(content.x + content.width - 1, content.y, 1, content.height), border);
            EditorGUI.DrawRect(new Rect(content.x, content.height + content.y - 1, content.width, 1), border);

            this.editor?.OnInspectorGUI();

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        });

        //EditorGUI.PropertyField(new Rect(position.x + 16, position.y, position.width - 16, position.height), property, label);

        //if (property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none))
        //{
        //    EditorGUI.indentLevel++;

        //    var content = EditorGUILayout.BeginVertical(GUI.skin.box);

        //    if (this.editor == null)
        //    {
        //        UnityEditor.Editor.CreateCachedEditor(property.objectReferenceValue, null, ref this.editor);
        //    }

        //    EditorGUI.DrawRect(new Rect(content.x, content.y, content.width, 1), Color.black);
        //    EditorGUI.DrawRect(new Rect(content.x, content.y, 1, content.height), Color.black);
        //    EditorGUI.DrawRect(new Rect(content.x + content.width - 1,  content.y, 1, content.height), Color.black);
        //    EditorGUI.DrawRect(new Rect(content.x, content.height + content.y - 1, content.width, 1), Color.black);

        //    this.editor?.OnInspectorGUI();

        //    EditorGUILayout.EndVertical();
        //    EditorGUI.indentLevel--;
        //}
    }
}