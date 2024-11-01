using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Sprite))]
[CustomPropertyDrawer(typeof(Texture2D))]
public class TexturePreviewPropertyDrawer : BasePropertyDrawer
{
    private bool showPreview;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var baseHeight = base.GetPropertyHeight(property, label);

        if (this.showPreview)
        {
            return baseHeight + DefaultTextureSize + Gui.RowSpacing;
        }

        return baseHeight;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        this.FoldableContent(rect, property, label, ref this.showPreview, rect =>
        {
            this.Grid(new Rect(rect.x, rect.y, DefaultTextureSize, DefaultTextureSize));

            if (property.objectReferenceValue is Sprite sprite)
            {
                this.Texture(new Rect(rect.x, rect.y, DefaultTextureSize, DefaultTextureSize), sprite, false);
            }
            else if (property.objectReferenceValue is Texture2D texture)
            {
                this.Texture(new Rect(rect.x, rect.y, DefaultTextureSize, DefaultTextureSize), texture, false);
            }
        });
    }
}

