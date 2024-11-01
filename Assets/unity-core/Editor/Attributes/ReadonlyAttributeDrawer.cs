using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadonlyAttribute))]
public class ReadonlyAttributeDrawer : BasePropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        this.Readonly(() => base.OnGUI(position, property, label));
    }
}
