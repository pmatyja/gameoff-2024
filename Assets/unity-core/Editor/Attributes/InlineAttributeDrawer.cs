using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(InlineAttribute))]
public class InlineAttributeDrawer : BasePropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0.0f;

        foreach (var field in this.fieldInfo.FieldType.GetSerializableFields())
        {
            var relativeProperty = property.FindPropertyRelative(field.Name);
            if (relativeProperty == null)
            {
                Debug.LogWarning($"FindPropertyRelative method failed to retrive '{field.Name}' field. Skipping...");
                continue;
            }

            height += base.GetPropertyHeight(relativeProperty, label) + Gui.RowSpacing;
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        foreach (var field in this.fieldInfo.FieldType.GetSerializableFields())
        {
            var relativeProperty = property.FindPropertyRelative(field.Name);
            if (relativeProperty == null)
            {
                Debug.LogWarning($"FindPropertyRelative method failed to retrive '{field.Name}' field. Skipping...");
                continue;
            }

            float height = base.GetPropertyHeight(relativeProperty, label);

            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, height), relativeProperty);

            position.y += height + Gui.RowSpacing;
        }
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();

        foreach (var field in this.fieldInfo.FieldType.GetSerializableFields())
        {
            var relativeProperty = property.FindPropertyRelative(field.Name);
            if (relativeProperty == null)
            {
                Debug.LogWarning($"FindPropertyRelative method failed to retrive '{field.Name}' field. Skipping...");
                continue;
            }

            var fieldElement = new PropertyField();

            fieldElement.BindProperty(relativeProperty);

            root.Add(fieldElement);
        }

        return root;
    }
}