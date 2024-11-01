using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ColorTagAttribute))]
public class ColorTagAttributeDrawer : BasePropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (this.fieldInfo.TryGetAttribute<ColorTagAttribute>(out var attribute))
        {
            var tagRect = new Rect(position.x, position.y, 10.0f, position.height);

            EditorGUI.DrawRect(tagRect, Color.black);
            EditorGUI.DrawRect(new Rect(tagRect.x + 1, tagRect.y + 1, tagRect.width - 2, tagRect.height - 2), attribute.Color);

            position.x += 13.0f;
            position.width -= 13.0f;

            base.OnGUI(position, property, label);
        }
    }
}