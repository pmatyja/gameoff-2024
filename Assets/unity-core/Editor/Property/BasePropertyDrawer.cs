using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BasePropertyDrawer : PropertyDrawer
{
    protected const int DefaultTextureSize = 128;
    protected const float FoldoutSpacing = 16.0f;

    protected static VisualElement CreateProperty(string name, Action<VisualElement, VisualElement> onItem)
    {
        var root = new VisualElement();
        {
            root.style.flexDirection = FlexDirection.Row;

            var label = new Label(name);
            {
                label.AddToClassList("unity-base-field__label");
                label.AddToClassList("unity-property-field__label");
            }

            var input = new VisualElement();
            {
                input.AddToClassList("unity-base-field__input");
                input.AddToClassList("unity-property-field__input");
            }

            onItem.Invoke(label, input);

            root.Add(label);
            root.Add(input);
        }

        return root;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUI.GetPropertyHeight(property, label);

        //if (property.propertyPath.EndsWith("]"))
        //{
        //    baseHeight += Gui.LineHeight;
        //}

        if (this.fieldInfo.TryGetAttribute<BaseAttribute>(out var attribute))
        {
            if (attribute.Label == LabelState.SeparateLine)
            {
                height += Gui.NextLine;
            }
        }

        return height;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        this.OnGUI(rect, property, label, (content, property, label) => EditorGUI.PropertyField(content, property, label));
    }

    public void OnGUI(Rect content, SerializedProperty property, GUIContent label, Action<Rect, SerializedProperty, GUIContent> onDraw)
    {
        onDraw.Invoke(this.HandleLabel(content, property, label), property, GUIContent.none);
    }

    public Rect HandleLabel(Rect content, SerializedProperty property, GUIContent label)
    {
        if (this.fieldInfo != null)
        {
            if (this.fieldInfo.TryGetAttribute<BaseAttribute>(out var attribute))
            {
                if (attribute.Label == LabelState.Hidden)
                {
                    return content;
                }

                if (attribute.Label == LabelState.SeparateLine)
                {
                    var rect = new Rect(content.x, content.y, content.width, EditorGUI.GetPropertyHeight(property, label));

                    EditorGUI.LabelField(rect, label);

                    rect.y += Gui.NextLine;
                    rect.height -= Gui.NextLine;

                    return rect;
                }
            }
        }

        return EditorGUI.PrefixLabel(content, label);
    }

    protected void Readonly(Action action)
    {
        var previousGUIState = GUI.enabled;
        GUI.enabled = false;

        action.Invoke();

        GUI.enabled = previousGUIState;
    }

    protected void BackgroundColor(Color color, Action action)
    {
        var oldColor = GUI.backgroundColor;

        GUI.backgroundColor = color;
        action.Invoke();
        GUI.backgroundColor = oldColor;
    }

    protected Rect StateButton(Rect rect, bool state, string active, string inactive, Action<bool> onClick)
    {
        this.BackgroundColor(state ? Color.green : Color.red, () =>
        {
            if (GUI.Button(new Rect(rect.x, rect.y, rect.width, Gui.LineHeight), new GUIContent(state ? active : inactive)))
            {
                onClick.Invoke(state);
            }
        });

        rect.y += Gui.NextLine;
        rect.height -= Gui.NextLine;

        return rect;
    }

    protected void Grid(Rect rect, int blockSize = 32)
    {
        var cols = rect.width / blockSize;
        var rows = rect.height / blockSize;

        for (var y = 0; y < rows; ++y)
        {
            for (var x = 0; x < cols; ++x)
            {
                var color = ( ( x % 2 + y % 2 ) % 2 ) == 0 ? 0.78f : 0.98f;

                EditorGUI.DrawRect(new Rect(rect.x + x * blockSize, rect.y + y * blockSize, blockSize, blockSize), new(color, color, color));
            }
        }
    }

    protected void Texture(Rect rect, Texture2D texture, bool backgroundGrid = true)
    {
        if (backgroundGrid)
        {
            this.Grid(rect);
        }

        if (texture != null)
        {
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
        }
    }

    protected void Texture(Rect rect, Sprite sprite, bool backgroundGrid = true)
    {
        if (backgroundGrid)
        {
            this.Grid(rect);
        }

        if (sprite != null && sprite.texture != null)
        {
            var coords = new Rect
            (
                sprite.rect.x       / sprite.texture.width, 
                sprite.rect.y       / sprite.texture.height, 
                sprite.rect.width   / sprite.texture.width, 
                sprite.rect.height  / sprite.texture.height
            );

            GUI.DrawTextureWithTexCoords(rect, sprite.texture, coords);
        }
    }

    protected void FoldableContent(Rect rect, SerializedProperty property, GUIContent label, Action<Rect> onContent)
    {
        rect = this.HandleLabel(rect, property, label);
        
        EditorGUI.PropertyField(new Rect(rect.x + FoldoutSpacing, rect.y, rect.width - FoldoutSpacing, Gui.LineHeight), property, GUIContent.none);

        property.isExpanded = EditorGUI.Foldout(new Rect(rect.x, rect.y, FoldoutSpacing, Gui.LineHeight), property.isExpanded, GUIContent.none);

        if (property.isExpanded)
        {
            if (property.IsArray() == false)
            {
                rect.width -= Gui.LineHeight;
            }

            onContent.Invoke(new Rect(rect.x + FoldoutSpacing, rect.y + Gui.NextLine, rect.width - FoldoutSpacing, Gui.NextLine));
        }
    }

    protected void FoldableContent(Rect rect, SerializedProperty property, GUIContent label, ref bool isExpanded, Action<Rect> onContent)
    {
        rect = this.HandleLabel(rect, property, label);

        EditorGUI.PropertyField(new Rect(rect.x + FoldoutSpacing, rect.y, rect.width - FoldoutSpacing, Gui.LineHeight), property, GUIContent.none);

        isExpanded = EditorGUI.Foldout(new Rect(rect.x, rect.y, FoldoutSpacing, Gui.LineHeight), isExpanded, GUIContent.none);

        if (isExpanded)
        {
            if (property.IsArray() == false)
            {
                rect.width -= Gui.LineHeight;
            }

            onContent.Invoke(new Rect(rect.x + FoldoutSpacing, rect.y + Gui.NextLine, rect.width - FoldoutSpacing, Gui.NextLine));
        }
    }
}
