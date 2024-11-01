using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Graphs
{
    public class GraphNode : UnityEditor.Experimental.GraphView.Node
    {
        private readonly NodeGraphView graph;
        private readonly NodeInfo instance;
        private readonly SerializedProperty serializedProperty;
        private readonly Type type;
        
        public GraphNode(NodeGraphView graph, NodeInfo instance, SerializedProperty serializedProperty)
        {
            this.graph = graph;
            this.instance = instance;
            this.serializedProperty = serializedProperty;
            this.type = instance.Properties.GetType();

            var image = EditorOnly.GetIcon(this.type);
            if (image != null)
            {
                if (this.titleContainer.TryGet<Label>(out var name))
                {
                    name.style.paddingLeft = 20.0f;
                    name.AddChild<VisualElement>(frame =>
                    {
                        frame.style.left = -20.0f;
                        frame.style.justifyContent = Justify.Center;
                        frame.style.height = new StyleLength(new Length(100.0f, LengthUnit.Percent));

                        frame.AddChild<VisualElement>(icon =>
                        {
                            icon.style.width = 16.0f;
                            icon.style.height = 16.0f;
                            icon.style.backgroundImage = new StyleBackground(image);
                        });
                    });
                }
            }

            this.Initialize();
            this.AddPorts();
            this.AddBody();
        }

        public Port CreatePort(NodePortInfo info)
        {
            var port = this.InstantiatePort(Orientation.Horizontal, info.Input ? Direction.Input : Direction.Output, info.Single ? Port.Capacity.Single : Port.Capacity.Multi, info.Type);

            port.name = $"{this.instance.Id}.{info.Name}";
            port.portName = info.Name;
            port.portColor = info.Color;
            port.userData = info;

            if (info.Input)
            {
                this.inputContainer.Add(port);
            }
            else
            {
                this.outputContainer.Add(port);
            }

            return port;
        }

        private void Initialize()
        {
            this.name = this.instance.Id;
            this.title = NodeGraphCache.GetNodeName(this.type);
            this.style.width = this.instance.Properties.Width;
            this.capabilities = Capabilities.Selectable | Capabilities.Movable | Capabilities.Ascendable |
                                Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable |
                                Capabilities.Deletable;
            this.userData = this.instance;

            this.SetPosition(new Rect(this.instance.Position.x, this.instance.Position.y, 64, 64));

            // Title

            if (ColorUtility.TryParseHtmlString(this.instance.Properties.BackgroundColor, out var backgroundColor))
            {
                this.titleContainer.style.backgroundColor = backgroundColor;
            }

            this.titleButtonContainer?.RemoveFromHierarchy();
        }
        
        private void AddPorts()
        {
            foreach (var info in NodeGraphCache.GetPorts(this.type))
            {
                this.CreatePort(info);
            }

            this.RefreshPorts();
        }

        private void AddBody()
        {
            var root = new VisualElement
            {
                style =
                {
                    paddingLeft = new StyleLength(new Length(5, LengthUnit.Pixel)),
                    paddingRight = new StyleLength(new Length(5, LengthUnit.Pixel)),
                    paddingTop = new StyleLength(new Length(5, LengthUnit.Pixel)),
                    paddingBottom = new StyleLength(new Length(5, LengthUnit.Pixel))
                }
            };

            foreach (var field in this.type.GetSerializableFields())
            {
                if (field.HasAttribute<ParameterAttribute>())
                {
                    continue;
                }

                if (field.HasAttribute<OutputAttribute>())
                {
                    continue;
                }

                var relativeProperty = this.serializedProperty.FindPropertyRelative(field.Name);
                if (relativeProperty == null)
                {
                    continue;
                }

                var property = new PropertyField();

                property.BindProperty(relativeProperty);

                root.Add(property);
            }

            if (root.childCount > 0)
            {
                this.extensionContainer.Add(root);
                this.RefreshExpandedState();
            }

            this.extensionContainer.style.backgroundColor = new Color(0.24f, 0.24f, 0.24f, 0.8f);
        }
    }
}