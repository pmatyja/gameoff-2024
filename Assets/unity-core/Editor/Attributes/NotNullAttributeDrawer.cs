using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NotNullAttribute))]
public class NotNullAttributeDrawer : BasePropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.objectReferenceValue == null)
        {
            return base.GetPropertyHeight(property, label) + Gui.LineHeight * 2.0f + Gui.RowSpacing;
        }

        return base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.objectReferenceValue == null)
        {
            var message = (this.attribute as NotNullAttribute)?.Message;

            if (string.IsNullOrWhiteSpace(message))
            {
                message = $"Property '{property.name}' cannot be null";
            }

            EditorGUI.HelpBox(new Rect(position.x, position.y, position.width, Gui.LineHeight * 2.0f), message, MessageType.Error);

            position.y += Gui.LineHeight * 2.0f + Gui.RowSpacing;
        }

        base.OnGUI(position, property, label);
    }
}