using UnityEditor;
using UnityEngine;

public class RuntimePreviewPropertyDrawer : BasePropertyDrawer
{
    public virtual int Size { get; } = 128;
    public virtual Vector3 Pivot { get; } = Vector3.zero;
    public virtual Quaternion Rotation { get; protected set; }

    private bool showPreview;

    private static readonly GUIStyle Style = new()
    {
        normal = new GUIStyleState
        {
            background = EditorGUIUtility.whiteTexture
        }
    };

    private UnityEditor.Editor preview;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var baseHeight = base.GetPropertyHeight(property, label);

        if (property.propertyPath.EndsWith("]"))
        {
            baseHeight += Gui.LineHeight;
        }

        if (this.showPreview)
        {
            if (this.GetModel(property) != null)
            {
                return baseHeight + this.Size + Gui.RowSpacing;
            }
        }

        return baseHeight;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        this.FoldableContent(rect, property, label, ref this.showPreview, (content) => 
        {
            if (this.GetModel(property) is GameObject model)
            {
                var texture = RuntimePreviewGenerator.GenerateModelPreview(model, this.GetRotation(property), this.GetRotationPivot(property), this.Size, this.Size);

                content.x -= EditorGUIUtility.labelWidth;

                if (texture != null)
                {
                    GUI.DrawTexture(new Rect(content.x, content.y, this.Size, this.Size), texture);
                }

                if (model == null)
                {
                    this.preview = null;
                }
                else
                {
                    if (this.preview == null)
                    {
                        this.preview = UnityEditor.Editor.CreateEditor(model);
                    }

                    this.preview.OnInteractivePreviewGUI(new Rect(content.x + this.Size + 4.0f, content.y, content.width - (this.Size + 4.0f), this.Size), Style);
                }
            }
        });
    }

    public virtual GameObject GetModel(SerializedProperty property)
    {
        if (property.type.Contains("PrefabPair"))
        {
            if (property.objectReferenceValue is PrefabSO prefab)
            {
                return prefab?.Model;
            }
        }

        if (property.type.Contains("PPtr<$GameObject>") == false)
        {
            return null;
        }

        return property.objectReferenceValue as GameObject;
    }

    public virtual Quaternion GetRotation(SerializedProperty property)
    {
        if (property.serializedObject.targetObject is PrefabSO prefab)
        {
            return prefab.GetQuaternion();
        }

        return this.Rotation;
    }

    public virtual Vector3 GetRotationPivot(SerializedProperty property)
    {
        if (property.serializedObject.targetObject is PrefabSO prefab)
        {
            return prefab.Pivot;
        }

        return this.Pivot;
    }
}
