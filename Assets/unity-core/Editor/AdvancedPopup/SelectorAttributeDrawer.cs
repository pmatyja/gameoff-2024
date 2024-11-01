using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SelectorAttribute))]
[CustomPropertyDrawer(typeof(string))]
public class SelectorAttributeDrawer : BasePropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = base.GetPropertyHeight(property, label) + Gui.RowSpacing;

        if (this.fieldInfo.TryGetAttribute<SelectorAttribute>(out var selector))
        {
            if (selector.AllowManualEdit)
            {
                height += Gui.NextLine;
            }
        }

        return height;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        if (this.fieldInfo.TryGetAttribute<SelectorAttribute>(out var selector))
        {
            AdvancedPopup.Draw(rect, property, label, property.GetParentInstance(), this.fieldInfo.GetReference(), selector);
        }
        else
        { 
            base.OnGUI(rect, property, label);
        }
    }
}
