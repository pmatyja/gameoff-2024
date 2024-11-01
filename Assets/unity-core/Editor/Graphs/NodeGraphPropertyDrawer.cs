using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

[CustomPropertyDrawer(typeof(NodeGraph))]
public class NodeGraphPropertyDrawer : BasePropertyDrawer
{
    private static readonly Color CreateColor = new Color(0.397f, 0.0f, 0.0f);
    private static readonly Color OpenColor = new Color(0.0f, 0.397f, 0.0f);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var rect = EditorGUI.PrefixLabel(position, label);

        if (property.propertyType != SerializedPropertyType.ManagedReference)
        {
            Debug.LogError("Graph is invalid or property is missing 'SerializableReference' attribute");
            EditorGUI.LabelField(rect, "Graph is invalid or property is missing 'SerializableReference' attribute");
            return;
        }

        var text  = property.managedReferenceValue == null ? "Create Graph" : "Open Graph";
        var width = rect.width - 24.0f - Gui.ColumnSpacing;

        var oldColor = GUI.backgroundColor;
        GUI.backgroundColor = property.managedReferenceValue == null ? Color.red : Color.green;

        if (GUI.Button(new Rect(rect.x, rect.y, width, rect.height), new GUIContent(text)))
        {
            NodeGraphEditorWindow.OpenGraph(property, typeof(INode));
        }

        GUI.backgroundColor= oldColor;

        if (GUI.Button(new Rect(rect.x + width + Gui.ColumnSpacing, rect.y, 24.0f, rect.height), new GUIContent(EditorOnly.GetIcon("d_winbtn_win_close"))))
        {
            if (property.managedReferenceValue is NodeGraph graph)
            {
                graph.Clear();
                property.managedReferenceValue = null;
            }
        }
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return CreateProperty(property.name, (label, input) =>
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                Debug.LogError("Graph is invalid or property is missing 'SerializableReference' attribute");
                input.Add(new Label("Graph is invalid or property is missing 'SerializableReference' attribute"));
                return;
            }

            input.style.flexDirection = FlexDirection.Row;

            var openOrCreate = new Button
            {
                text = property.managedReferenceValue == null ? "Create Graph" : "Open Graph",
                style =
                {
                    backgroundColor = GUI.backgroundColor = property.managedReferenceValue == null ? CreateColor : OpenColor
                }
            };

            openOrCreate.style.flexGrow = 2;
            openOrCreate.OnClick(_ =>
            {
                NodeGraphEditorWindow.OpenGraph(property, typeof(INode));

                openOrCreate.text = "Open Graph";
                openOrCreate.style.backgroundColor = OpenColor;
            });

            var remove = new Button { text = "X" };

            remove.style.minWidth = new StyleLength(new Length(24, LengthUnit.Pixel));
            remove.OnClick(_ =>
            {
                if (property.managedReferenceValue is NodeGraph graph)
                {
                    graph.Clear();
                    property.managedReferenceValue = null;
                    NodeGraphEditorWindow.ClearGraph();

                    openOrCreate.text = "Create Graph";
                    openOrCreate.style.backgroundColor = CreateColor;
                }
            });

            input.Add(openOrCreate);
            input.Add(remove);
        });
    }
}