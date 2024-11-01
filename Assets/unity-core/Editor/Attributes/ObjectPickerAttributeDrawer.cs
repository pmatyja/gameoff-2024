using System.Collections.Concurrent;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ObjectPickerAttribute))]
public class ObjectPickerAttributeDrawer : BasePropertyDrawer
{
    private readonly static ConcurrentDictionary<int, SerializedProperty> PickerIDs = new();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + Gui.NextLine;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        // Clear cache just in case of too many entries

        if (PickerIDs.Count > 50)
        {
            PickerIDs.Clear();
        }

        if (this.attribute is ObjectPickerAttribute picker)
        {
            var type = picker.ObjectType;

            if (type == null)
            {
                var instance = property.GetParentInstance();

                type = instance.GetTypeSource(picker.ObjectTypeSource);
            }

            base.OnGUI(rect, property, label, (rect, property, label) =>
            {
                this.Readonly(() =>
                {
                    EditorGUI.TextField(rect, label, property.objectReferenceValue?.ToString() ?? $"Nothing ({type?.GetFormattedName()})");
                });
            });

            var id = EditorGUIUtility.GetControlID(FocusType.Passive);

            if (GUI.Button(new Rect(rect.x, rect.y + Gui.NextLine, rect.width, Gui.NextLine), new GUIContent("Pick")))
            {
                var methodType = typeof(EditorGUIUtility).GetMethod(nameof(EditorGUIUtility.ShowObjectPicker), BindingFlags.Static | BindingFlags.Public);

                var genericMethod = methodType.MakeGenericMethod(type);
                if (genericMethod != null)
                {
                    genericMethod.Invoke(null, new object[] { null, picker.AllowSceneObjects, picker.SearchFilter, id });

                    PickerIDs.TryAdd(id, property);
                }
            }

            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                if (PickerIDs.TryRemove(id, out var result))
                {
                    result.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                }
            }
        }
    }
}
