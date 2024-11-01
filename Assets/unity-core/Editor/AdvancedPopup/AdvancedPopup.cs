using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class AdvancedPopup : AdvancedDropdown
{
    private const int MaxLineCount = 13;
    private const string Nothing = "<Nothing>";

    private readonly ReferenceInfo context;
    private readonly object parent;
    private readonly SelectorAttribute selector;
    private readonly Action<object> onSelected;

    private AdvancedPopup
    (
        ReferenceInfo context,
        object parent,
        SelectorAttribute selector,
        Action<object> onSelected
    )
        : base(new AdvancedDropdownState())
    {
        this.minimumSize = new Vector2(this.minimumSize.x, Gui.LineHeight * MaxLineCount + Gui.LineHeight * 2f);

        this.context = context;
        this.parent = parent;
        this.selector = selector;
        this.onSelected = onSelected;
    }

    public static void Popup(Vector2 position, ReferenceInfo context, object parent, SelectorAttribute itemHandler, Action<object> onSelected)
    {
        new AdvancedPopup(context, parent, itemHandler, selected =>
        {
            if (selected == null)
            {
                return;
            }

            onSelected.Invoke(selected);
        })
        .Show(new Rect(position.x, position.y, 256, 0));
    }

    public static void Draw
    (
        Rect rect, 
        SerializedProperty property,
        GUIContent label,
        object parent,
        ReferenceInfo fieldInfo,
        SelectorAttribute selector, 
        Action<Rect> onDraw = null,
        Action<object> onSelected = null
    )
    {
        EditorGUI.BeginProperty(rect, null, property);

        var baseType = fieldInfo.Type;
        var value = fieldInfo.GetValue(parent);

        if (value is IEnumerable<object> items)
        {
            var lastOpeningIndex = property.propertyPath.IndexOf('[');
            if (lastOpeningIndex > -1)
            {
                var lastClosingIndex = property.propertyPath.IndexOf("]");
                if (lastClosingIndex > -1)
                {
                    var index = int.Parse(property.propertyPath.Substring(lastOpeningIndex + 1, lastClosingIndex - lastOpeningIndex - 1));

                    baseType = fieldInfo?.Type.GetGenericArguments()[0];
                    value = items.Skip(index).FirstOrDefault();
                }
            }
        }

        var selectedItem = selector.GetSelectedItem(value);

        if (string.IsNullOrWhiteSpace(selectedItem))
        {
            selectedItem = Nothing;
        }

        if (fieldInfo.Type.IsArray())
        {
            rect = new Rect(rect.x + 6.0f, rect.y, rect.width - 6.0f, rect.height);
        }
        else
        {
            if (selector.Label == LabelState.SeparateLine)
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, Gui.NextLine), label);
                rect.y += Gui.NextLine;
            }
            else if (selector.Label == LabelState.Default)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }
        }

        if (selector.AllowManualEdit)
        {
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, Gui.LineHeight), property, GUIContent.none, true);

            rect.y += Gui.NextLine;
        }

        if (EditorGUI.DropdownButton(new Rect(rect.x, rect.y, rect.width, Gui.NextLine), new GUIContent(selectedItem), FocusType.Keyboard))
        {
            new AdvancedPopup(fieldInfo, parent, selector, value =>
            {
                if (property.propertyType == SerializedPropertyType.ManagedReference || property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    property.managedReferenceValue = value;
                    property.isExpanded = property.managedReferenceValue != null;
                }
                else if (property.propertyType == SerializedPropertyType.String)
                {
                    property.stringValue = value?.ToString();
                }

                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            })
            .Show(rect);
        }

        if (property.propertyType == SerializedPropertyType.ManagedReference || property.propertyType == SerializedPropertyType.ObjectReference)
        {
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUI.GetPropertyHeight(property)), property, GUIContent.none, true);
        }

        if (onDraw != null)
        {
            onDraw?.Invoke(rect);
        }

        EditorGUI.EndProperty();
    }

    protected override AdvancedDropdownItem BuildRoot()
	{
        var root = new AdvancedPopupGroup(0, string.Empty);

        root.AddChild(new AdvancedPopupItem(0, null, Nothing, default));

        var items = this.selector.GetItems(this.context, this.parent);

        foreach (var item in items)
        {
            var name = NormalizeName(this.selector.GetItemName(item));

            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            var group = NormalizeGroup(this.selector.GetItemGroup(item));

            var childRoot = AdvancedPopupGroup.GetOrAdd(root, group);

            childRoot.AddChild(new AdvancedPopupItem(root.children.Count(), this.selector.GetItemIcon(item), name, item));
        }

        root.name = "Select";

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        base.ItemSelected(item);

        if (item is AdvancedPopupItem selectedItem)
        {
            var value = selector.GetValue(selectedItem.Value);

            this.onSelected?.Invoke(value);
        }
    }

    protected static string NormalizeGroup(string item, string defaultGroup = "")
    {
        if (string.IsNullOrWhiteSpace(item))
        {
            return defaultGroup;
        }

        var group = item.Replace('.', '/')?.Replace('\\', '/') ?? defaultGroup;

        if (string.IsNullOrWhiteSpace(group) == false)
        {
            if (group.EndsWith('/') == false)
            {
                group += '/';
            }
        }

        if (string.IsNullOrWhiteSpace(group))
        {
            return defaultGroup;
        }

        return group;
    }

    protected static string NormalizeName(string item)
    {
        if (string.IsNullOrWhiteSpace(item))
        {
            return null;
        }

        var slash = item.LastIndexOf('/');
        if (slash > -1)
        {
            item = item.Substring(slash + 1);
        }

        var backslash = item.LastIndexOf('\\');
        if (backslash > -1)
        {
            item = item.Substring(backslash + 1);
        }

        return item.TrimEnd('/', '\\');
    }

    protected static void Readonly(Action action)
    {
        var previousGUIState = GUI.enabled;
        GUI.enabled = false;

        action.Invoke();

        GUI.enabled = previousGUIState;
    }
}