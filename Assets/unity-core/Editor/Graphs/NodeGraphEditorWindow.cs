using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeGraphEditorWindow : EditorWindow
{
    private Toolbar toolbar;
    private NodeGraphView graphView;
    private int instanceId;

    public NodeGraphEditorWindow()
    {
        this.titleContent = new GUIContent("Node Graph");
    }

    public static void OpenGraph(SerializedProperty serializedProperty, Type baseType)
    {
        GetWindow<NodeGraphEditorWindow>().LoadGraph(serializedProperty.serializedObject.targetObject, serializedProperty.name, baseType);
    }

    public static void ClearGraph()
    {
        GetWindow<NodeGraphEditorWindow>().CloseGraph();
    }

    private void LoadGraph(UnityEngine.Object unityObject, string fieldName, Type baseType)
    {
        this.graphView?.RemoveFromHierarchy();

        if (unityObject == null)
        {
            return;
        }

        this.instanceId = unityObject.GetInstanceID();
        this.graphView = new NodeGraphView(unityObject, fieldName, baseType);

        this.rootVisualElement.Add(this.graphView);

        this.graphView.StretchToParentSize();

        this.toolbar?.BringToFront();

        this.Show();
        this.Focus();
    }

    private void CloseGraph()
    {
        this.toolbar?.RemoveFromHierarchy();
        this.toolbar = null;

        this.graphView?.RemoveFromHierarchy();
        this.graphView = null;

        this.instanceId = 0;
    }

    private void OnEnable()
    {
        this.toolbar = new Toolbar
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                flexGrow = 0,
                backgroundColor = new Color(0.25f, 0.25f, 0.25f, 0.75f)
            }
        };

        var options = new VisualElement
        {
            style = { alignContent = Align.Center }
        };

        this.toolbar.Add(options);

        var searchField = new ToolbarSearchField();
        {
            searchField.RegisterValueChangedCallback(evt =>
            {
                if (this.graphView == null)
                {
                    return;
                }

                if (string.IsNullOrEmpty(evt.newValue))
                {
                    this.graphView.FrameAll();
                    return;
                }

                this.graphView.ClearSelection();
                this.graphView.graphElements.ForEach(x =>
                {
                    if (x is UnityEditor.Experimental.GraphView.Node node && x.title.IndexOf(evt.newValue, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        this.graphView.AddToSelection(node);
                    }
                });

                this.graphView.FrameSelection();
            });

            this.toolbar.Add(searchField);
        }

        var findGraph = new Button();
        {
            findGraph.text = "Find Graph In Editor";
            findGraph.OnClick(_ =>
            {
                if (this.instanceId != 0)
                {
                    EditorGUIUtility.PingObject(this.instanceId);
                    Selection.activeInstanceID = this.instanceId;
                }
            });

            this.toolbar.Add(findGraph);
        }

        this.rootVisualElement.Add(this.toolbar);
    }

    private void OnDisable()
    {
        this.CloseGraph();
    }
}
