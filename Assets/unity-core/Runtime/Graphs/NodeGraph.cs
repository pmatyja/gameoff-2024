using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class NodeGraph
{
    [SerializeField]
    private string id = Guid.NewGuid().ToString();
    public string Id => this.id;

    [SerializeField]
    [Readonly]
    private Vector3 editorPosition;
    public Vector3 EditorPosition => this.editorPosition;

    [SerializeField]
    [Readonly]
    private Vector3 editorScale = Vector3.one;
    public Vector3 EditorScale => this.editorScale;

    [SerializeField]
    private List<NodeInfo> nodes = new()
    {
        new NodeInfo
        {
            Position = new Vector2(32, 32),
            Properties = new StartNode()
        }
    };
    public IReadOnlyList<NodeInfo> Nodes => this.nodes;

    [SerializeField]
    private List<NodeConnection> connections = new();
    public IReadOnlyList<NodeConnection> Connections => this.connections;

    private bool isDirty = true;
    private StartNode start;

    public void Clear()
    {
        this.editorPosition = Vector3.zero;
        this.editorScale = Vector3.one;

        this.nodes = new()
        {
            new NodeInfo
            {
                Position = new Vector2(32, 32),
                Properties = new StartNode()
            }
        };

        this.connections.Clear();
    }

    public void AddNode(NodeInfo node)
    {
        this.nodes.Add(node);
        this.isDirty = true;
    }

    public void RemoveNode(NodeInfo node)
    {
        this.nodes.Remove(node);
        this.connections = this.connections.Where(connection => connection.OutputNode != node.Id && connection.InputNode != node.Id).ToList();
        this.isDirty = true;
    }

    public void AddConnection(NodeConnection connection)
    {
        this.connections.Add(connection);
        this.isDirty = true;
    }

    public void RemoveConnection(NodeConnection connection)
    {
        this.connections.Remove(connection);
        this.isDirty = true;
    }

    public ICoroutineTask Schedule()
    {
        return CoroutineManager.Start(this.ExcuteAsync(new NodeGraphContext()), nameof(NodeGraph));
    }

    public IEnumerator ExcuteAsync(INodeGraphContext context)
    {
        this.Compile();

        if (this.start == null)
        {
            Debug.LogWarning("Graph has not Start or Event nodes");
            yield break;
        }

        yield return null;
        yield return this.start?.ExcuteAsync(context ?? new NodeGraphContext());

        GameManager.Mode = GameMode.Gameplay;
    }

    public void Compile()
    {
        if (this.isDirty == false)
        {
            return;
        }

        // Find start node

        this.start = this.nodes.FirstOrDefault(x => x.Properties is StartNode)?.Properties as StartNode;

        // Reset ports

        foreach (var node in this.Nodes)
        {
            foreach (var info in NodeGraphCache.GetPorts(node.GetType()))
            {
                info.Field?.SetValue(node.Properties, null);
            }
        }

        // Connect nodes

        foreach (var connection in this.Connections)
        {
            var from = this.nodes.FirstOrDefault(x => x.Id == connection.OutputNode);
            var to = this.nodes.FirstOrDefault(x => x.Id == connection.InputNode);

            if (connection.IsValueNode)
            {
                var parameter = NodeGraphCache.GetPort(to.Properties.GetType(), connection.InputPortName);
                
                parameter.Field.SetValue(to.Properties, from.Properties);

                continue;
            }

            var port = NodeGraphCache.GetPort(from.Properties?.GetType(), connection.OutputPortName);
            if (port == null)
            {
                continue;
            }

            if (port.Single)
            {
                port.Field.SetValue(from.Properties, to.Properties);
            }
            else
            {
                var list = port.Field.GetValue(from.Properties) as IList;
                if (list == null)
                {
                    list = Activator.CreateInstance(port.Field.FieldType) as IList;

                    port.Field.SetValue(from.Properties, list);
                }

                list.Add(to.Properties);
            }
        }

        this.isDirty = false;
    }

    public void OnEnable()
    {
        if (this.nodes == null)
        {
            return;
        }

        foreach (var node in this.nodes)
        {
            node?.OnEnable();
        }
    }

    public void OnDisable()
    {
        if (this.nodes == null)
        {
            return;
        }

        foreach (var node in this.nodes)
        {
            node?.OnDisable();
        }
    }
}
