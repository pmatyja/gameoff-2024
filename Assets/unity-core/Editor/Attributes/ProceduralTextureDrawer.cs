using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ProceduralTextureAttribute))]
public class ProceduralTextureDrawer : BasePropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (this.attribute as ProceduralTextureAttribute)?.Size ?? 64.0f;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        this.Texture(rect, property.objectReferenceValue as Texture2D, false);
    }
}