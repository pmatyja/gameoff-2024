using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
[CustomPropertyDrawer(typeof(HideIfAttribute))]
[CustomPropertyDrawer(typeof(EnableIfAttribute))]
[CustomPropertyDrawer(typeof(DisableIfAttribute))]
[CustomPropertyDrawer(typeof(DependsOnAttribute))]
public class DependsOnAttributeDrawer : BasePropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var attribute = this.attribute as DependsOnAttribute;

        if (attribute.Action == DependsOnAttribute.DependencyAction.Show)
        {
            if (property.ValidateValues(attribute.Field, attribute.Values) == false)
            {
                return 0.0f;
            }
        }
        else if (attribute.Action == DependsOnAttribute.DependencyAction.Hide)
        {
            if (property.ValidateValues(attribute.Field, attribute.Values))
            {
                return 0.0f;
            }
        }

        return base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attribute = this.attribute as DependsOnAttribute;
        var state = property.ValidateValues(attribute.Field, attribute.Values);

        if (attribute.Action == DependsOnAttribute.DependencyAction.Enable)
        {
            if (state)
            {
                base.OnGUI(position, property, label);
            }
            else
            {
                this.Readonly(() => base.OnGUI(position, property, label));
            }
        }
        else if (attribute.Action == DependsOnAttribute.DependencyAction.Disable)
        {
            if (state)
            {
                this.Readonly(() => base.OnGUI(position, property, label));
            }
            else
            {
                base.OnGUI(position, property, label);
            }
        }
        else if (attribute.Action == DependsOnAttribute.DependencyAction.Show)
        {
            if (state)
            {
                base.OnGUI(position, property, label);
            }
        }
        else if (attribute.Action == DependsOnAttribute.DependencyAction.Hide)
        {
            if (state == false)
            {
                base.OnGUI(position, property, label);
            }
        }
    }
}
