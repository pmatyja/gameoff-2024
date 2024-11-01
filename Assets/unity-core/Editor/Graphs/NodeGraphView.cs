using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Graphs;
using Nodes.Value;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeGraphView : GraphView
{
    private const string NodeGraphUndoName = "NodeGraph";

    private readonly UnityEngine.Object unityObject;
    private readonly string fieldName;
    private readonly Type baseType;

    private NodeGraph instance;

    public NodeGraphView(UnityEngine.Object unityObject, string fieldName, Type baseType)
    {
        this.unityObject = unityObject;
        this.fieldName = fieldName;
        this.baseType = baseType;

        this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var gridBackground = new GridBackground();

        this.Insert(0, gridBackground);

        gridBackground.StretchToParentSize();
        this.Build();
    }

    private SerializedProperty GetProperty()
    {
        return new SerializedObject(this.unityObject).FindProperty(this.fieldName);
    }

    private void Build()
    {
        foreach (var node in this.nodes)
        {
            this.RemoveElement(node);
        }

        var serializedProperty = this.GetProperty();
        if (serializedProperty == null)
        {
            Debug.LogError("Cannot load Graph instance");
            return;
        }

        if (serializedProperty.propertyType != SerializedPropertyType.ManagedReference)
        {
            Debug.LogError("Graph is invalid or property is missing 'SerializableReference' attribute");
            return;
        }

        if (serializedProperty.managedReferenceValue == null)
        {
            serializedProperty.managedReferenceValue = Activator.CreateInstance(typeof(NodeGraph)) as NodeGraph;

            serializedProperty.serializedObject.ApplyModifiedProperties();
            serializedProperty.serializedObject.Update();

            this.instance = serializedProperty.managedReferenceValue as NodeGraph;
            this.instance.Clear();

            this.UpdateChanges();
        }

        if (serializedProperty.FindPropertyRelative("nodes") is SerializedProperty graphNodes)
        {
            // Get root
            this.instance = serializedProperty.managedReferenceValue as NodeGraph;

            // Nodes

            for (var i = 0; i < graphNodes.arraySize; ++i)
            {
                if (this.instance.Nodes[i] == null)
                {
                    continue;
                }

                if (this.instance.Nodes[i].Properties == null)
                {
                    continue;
                }

                var item = graphNodes.GetArrayElementAtIndex(i);
                var node = item.FindPropertyRelative(nameof(NodeInfo.Properties));

                this.AddElement(new Editor.Graphs.GraphNode(this, this.instance.Nodes[i], node));
            }

            // Connections 

            foreach (var connection in this.instance.Connections)
            {
                var to = this.ports.FirstOrDefault(x => x.name == connection.InputPort);
                if (to == null)
                {
                    continue;
                }
                
                var from = this.ports.FirstOrDefault(x => x.name == connection.OutputPort);
                if (from == null)
                {
                    continue;
                }
                
                this.AddElement(from.ConnectTo(to));
            }
        }

        this.graphViewChanged = info =>
        {
            this.OnMovedElements(info.movedElements);
            this.OnRemovedElements(info.elementsToRemove);
            this.OnAddEdges(info.edgesToCreate);

            this.UpdateChanges();

            return info;
        };

        this.viewTransformChanged = info =>
        {
            var serializedProperty = this.GetProperty();

            serializedProperty.FindPropertyRelative("editorPosition").vector3Value = this.contentViewContainer.transform.position;
            serializedProperty.FindPropertyRelative("editorScale").vector3Value = this.contentViewContainer.transform.scale;

            serializedProperty.serializedObject.ApplyModifiedProperties();
        };

        this.UpdateViewTransform(this.instance.EditorPosition, Vector3.Max(this.instance.EditorScale, Vector3.one));
        this.FrameAll();
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        if (evt.target is GraphNode)
        {
            base.BuildContextualMenu(evt);
        }
        else
        {
            AdvancedPopup.Popup(evt.mousePosition, null, null, new NodeSelectorAttribute(this.baseType), selected =>
            {
                if (selected == null)
                {
                    return;
                }

                this.OnNodeAdded(selected as Type, evt.mousePosition);
            });
        }
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var results = new List<Port>();

        this.ports.ForEach(port =>
        {
            if (port == startPort || port.node == startPort.node || port.direction == startPort.direction)
            {
                return;
            }

            if (port.portType.IsAssignableFrom(startPort.portType))
            {
                results.Add(port);
            }
        });

        return results;
    }

    private void OnNodeAdded(Type type, Vector2 position)
    {
        if (Attribute.IsDefined(type, typeof(DisallowMultipleAttribute)))
        {
            var existingNode = this.instance.Nodes.FirstOrDefault(x => x.Properties?.GetType() == type);
            if (existingNode != null)
            {
                var element = this.nodes.FirstOrDefault(x => x.name == existingNode.Id);

                this.ClearSelection();
                this.AddToSelection(element);
                this.FrameSelection();

                Debug.LogWarning("Node already exists");
                return;
            }
        }

        var node = new NodeInfo
        {
            Position = new Vector2
            (
                position.x - (this.instance.EditorPosition.x * this.instance.EditorScale.x),
                position.y - (this.instance.EditorPosition.y * this.instance.EditorScale.x)
            ),
            Properties = Activator.CreateInstance(type) as INode
        };

        this.instance.AddNode(node);
        this.instance.Compile();

        var serializedProperty = this.GetProperty();

        var nodes = serializedProperty.FindPropertyRelative("nodes");
        if (nodes == null)
        {
            Debug.LogWarning("Unity disposed the Graph object :(");
            return;
        }

        var item = nodes.GetArrayElementAtIndex(nodes.arraySize - 1);

        this.AddElement(new Editor.Graphs.GraphNode(this, node, item.FindPropertyRelative(nameof(NodeInfo.Properties))));
        this.UpdateChanges();
    }

    private void OnRemovedElements(List<GraphElement> elements)
    {
        if (elements == null)
        {
            return;
        }

        foreach (var element in elements)
        {
            if (element is Edge edge)
            {
                var connection = this.instance.Connections.FirstOrDefault(x => x.OutputPort == edge.output.name && x.InputPort == edge.input.name);
                if (connection != null)
                {
                    this.instance.RemoveConnection(connection);
                }
            }
            else if (element is UnityEditor.Experimental.GraphView.Node node)
            {
                this.instance.RemoveNode(node.userData as NodeInfo);
            }
            else
            {
                Debug.Log($"Unhandeled removal of: '{element.name}' (Type: '{element.GetType().FullName}')");
            }
        }

        this.instance.Compile();
    }

    private void OnMovedElements(List<GraphElement> elements)
    {
        if (elements == null)
        {
            return;
        }

        foreach (var element in elements)
        {
            if (element is UnityEditor.Experimental.GraphView.Node node)
            {
                if (node.userData is NodeInfo info)
                {
                    var rect = node.GetPosition();
                    info.Position = new Vector2(rect.x, rect.y);
                }
            }
        }
    }

    private void OnAddEdges(List<Edge> edges)
    {
        if (edges == null)
        {
            return;
        }

        foreach (var edge in edges)
        {
            var output = edge.output.node.userData as NodeInfo;
            var input = edge.input.node.userData as NodeInfo;

            this.instance.AddConnection(new NodeConnection
            {
                IsValueNode = output.Properties is IValueNode,
                
                OutputNode = output.Id,
                OutputPort = edge.output.name,
                OutputPortName = edge.output.portName,

                InputNode = input.Id,
                InputPort = edge.input.name,
                InputPortName = edge.input.portName
            });
        }

        this.instance.Compile();
    }

    public void UpdateChanges()
    {
        var serializedProperty = this.GetProperty();

        Undo.RegisterCompleteObjectUndo(serializedProperty.serializedObject.targetObject, NodeGraphUndoName);
        EditorUtility.SetDirty(serializedProperty.serializedObject.targetObject);
    }
}
