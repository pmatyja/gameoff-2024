using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Nodes.Value;
using UnityEngine;

public static class NodeGraphCache
{
    private static readonly string ValueNodeName = typeof(ValueNode<>).Name;

    private static readonly Color FlowEdgeColor = new(193f / 255f, 172f / 255f, 77f / 255f);
    private static readonly Color DataEdgeColor = new(0.1803922f, 0.539f, 0.2235294f);

    public static string GetNodeName(Type type)
    {
        if (type == null)
        {
            return string.Empty;
        }

        var name = type.GetFormattedName();

        if (name.EndsWith("BehaviourNode"))
        {
            name = name.Substring(0, name.Length - "BehaviourNode".Length);
        }
        
        if (name.EndsWith("ValueNode"))
        {
            name = name.Substring(0, name.Length - "ValueNode".Length);
        }

        if (name.EndsWith("Node"))
        {
            name = name.Substring(0, name.Length - "Node".Length);
        }

        if (name.EndsWith("Event"))
        {
            name = name.Substring(0, name.Length - "Event".Length);
        }

        return EditorOnly.NicifyName(name);
    }

    public static string GetNodePath(Type type)
    {
        if (type == null)
        {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(type.Namespace))
        {
            return string.Empty;
        }

        return type.Namespace.Replace(".", "/").Replace("Nodes/", string.Empty);
    }

    public static IReadOnlyList<NodePortInfo> GetPorts(Type type)
    {
        var ports = new List<NodePortInfo>();

        if (type == null)
        {
            return ports;
        }

        if (type.TryGetAttribute<SingleInputAttribute>(out var single))
        {
            ports.Add(new NodePortInfo { Name = "In", Color = FlowEdgeColor, Type = single.Type, Input = true, Single = true, Field = null });
        }

        if (type.TryGetAttribute<MultipleInputsAttribute>(out var multiple))
        {
            ports.Add(new NodePortInfo { Name = "In", Color = FlowEdgeColor, Type = multiple.Type, Input = true, Single = false, Field = null });
        }

        if (typeof(IValueNode).IsAssignableFrom(type))
        {
            ports.Add(new NodePortInfo { Name = "Out", Color = DataEdgeColor, Type = GetBaseType(type), Input = false, Single = false, Field = null });
        }

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (field.TryGetAttribute<ParameterAttribute>(out var parameter))
            {
                var portType = field.FieldType.GetElementType() ?? field.FieldType.GenericTypeArguments.FirstOrDefault() ?? field.FieldType;

                ports.Add(new NodePortInfo
                { 
                    Name = $"{(parameter.IsOptional ? "[Optional] " : string.Empty)}{field.Name} ({portType.Name})",
                    Color = DataEdgeColor,
                    Type = portType, 
                    Input = true, 
                    Single = true, 
                    Field = field
                });
                continue;
            }

            if (field.TryGetAttribute<OutputAttribute>(out var output))
            {
                if (typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                {
                    ports.Add(new NodePortInfo
                    {
                        Name = field.Name,
                        Color = FlowEdgeColor,
                        Type = field.FieldType.GetElementType() ?? field.FieldType.GenericTypeArguments.FirstOrDefault(),
                        Input = false,
                        Single = false,
                        Field = field
                    });
                }
                else
                {
                    ports.Add(new NodePortInfo
                    {
                        Name = field.Name,
                        Color = FlowEdgeColor,
                        Type = field.FieldType,
                        Input = false,
                        Single = true,
                        Field = field
                    });
                }
            }
        }

        return ports;
    }

    public static NodePortInfo GetPort(Type type, string name)
    {
        return GetPorts(type).FirstOrDefault(x => x.Name == name);
    }

    private static Type GetBaseType(Type type)
    {
        if (type == null)
        {
            return null;
        }

        if (type.Name == ValueNodeName)
        {
            return type.GenericTypeArguments.FirstOrDefault() ?? type;
        }

        return GetBaseType(type?.BaseType);
    }
}
