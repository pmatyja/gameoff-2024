using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ScriptableObject))]
[CustomPropertyDrawer(typeof(IScriptableObjectAsset))]
[CustomPropertyDrawer(typeof(ResourceSO))]
[CustomPropertyDrawer(typeof(AudioResourceSO))]
[CustomPropertyDrawer(typeof(AmbientSO))]
[CustomPropertyDrawer(typeof(MusicSO))]
[CustomPropertyDrawer(typeof(SoundEffectSO))]
[CustomPropertyDrawer(typeof(VoiceLineSO))]
[CustomPropertyDrawer(typeof(VoiceSO))]
[CustomPropertyDrawer(typeof(ActorSO))]
[CustomPropertyDrawer(typeof(AgentPresetSO))]
public class ScriptableObjectPropertyDrawer : BasePropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = base.GetPropertyHeight(property, label);

        if (Attribute.IsDefined(fieldInfo.FieldType, typeof(ScriptableObjectAttribute)))
        {
            height += Gui.NextLine;
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = Gui.LineHeight;

        var rect = this.HandleLabel(position, property, label);

        EditorGUI.PropertyField(rect, property, GUIContent.none);

        if (fieldInfo.FieldType.TryGetAttribute<ScriptableObjectAttribute>(out var attribute))
        {
            rect.y += Gui.NextLine;
            rect = this.StateButton(rect, property.objectReferenceValue == null, "Create", "Remove", state =>
            {
                if (state)
                {
                    var instance = ScriptableObject.CreateInstance(fieldInfo.FieldType);
                    var filename = $"{attribute.AssetPrefix}_{property.serializedObject.targetObject.name}_{fieldInfo.Name}_{Guid.NewGuid().ToString().Substring(0, 6)}.asset";
                    var filepath = Path.Combine("Assets/Resources/Local", filename);

                    AssetDatabase.CreateAsset(instance, filepath);

                    property.objectReferenceValue = instance;
                    property.serializedObject.ApplyModifiedProperties();
                    property.serializedObject.Update();
                }
                else
                {
                    var filepath = AssetDatabase.GetAssetPath(property.objectReferenceValue.GetInstanceID());

                    // Do not remove shared resources. Just unassign them

                    if (filepath.StartsWith("Assets/Resources/Shared", StringComparison.OrdinalIgnoreCase))
                    {
                        property.objectReferenceValue = default;
                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.Update();

                        return;
                    }

                    if (EditorUtility.DisplayDialog("Are you sure?", $"Do you want to delete file: '{filepath}'", "Delete", "Keep"))
                    {
                        AssetDatabase.DeleteAsset(filepath);

                        property.objectReferenceValue = default;
                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.Update();
                    }
                }
            });
        }
    }
}