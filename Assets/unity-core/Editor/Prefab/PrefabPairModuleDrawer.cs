using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PrefabPair))]
public class PrefabPairModuleDrawer : BasePropertyDrawer
{
    private readonly int Size = 64;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return this.Size;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        var editMode = true;
        var isometricView = true;

        if (property.serializedObject.targetObject is PrefabGroupSO group)
        {
            isometricView = group.IsometricView;
            editMode = group.EditMode;
        }

        if (property.serializedObject.targetObject is PrefabSO prefab)
        {
            isometricView = prefab.IsometricView;
        }

        var adjacentPrefab = property.FindPropertyRelative(nameof(PrefabPair.AdjacentPrefab));
        var adjacentRotation = property.FindPropertyRelative(nameof(PrefabPair.AdjacentRotation));

        var centerPrefab = property.FindPropertyRelative(nameof(PrefabPair.CenterPrefab));
        var centerRotation = property.FindPropertyRelative(nameof(PrefabPair.CenterRotation));

        var left = RuntimePreviewGenerator.GenerateModelPreview
        (
            adjacentPrefab.objectReferenceValue as PrefabSO,
            adjacentRotation.intValue,
            this.Size,
            this.Size,
            isometricView ? null : RuntimePreviewGenerator.CameraPositionTop
        );

        this.Texture(new Rect(rect.x, rect.y, this.Size, this.Size), left);

        var right = RuntimePreviewGenerator.GenerateModelPreview
        (
            centerPrefab.objectReferenceValue as PrefabSO,
            centerRotation.intValue,
            this.Size,
            this.Size,
            isometricView ? null : RuntimePreviewGenerator.CameraPositionTop
        );

        this.Texture(new Rect(rect.x + this.Size + Gui.ColumnSpacing, rect.y, this.Size, this.Size), right);
    }
}