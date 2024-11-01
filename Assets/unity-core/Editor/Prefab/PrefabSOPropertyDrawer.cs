using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PrefabSO))]
public class PrefabSOPropertyDrawer : RuntimePreviewPropertyDrawer
{
    public override GameObject GetModel(SerializedProperty property)
    {
        if (property.objectReferenceValue is PrefabSO instance)
        {
            return instance.Model;
        }

        return base.GetModel(property);
    }

    public override Quaternion GetRotation(SerializedProperty property)
    {
        if (property.objectReferenceValue is PrefabSO instance)
        {
            return instance.GetQuaternion();
        }

        return base.GetRotation(property);
    }

    public override Vector3 GetRotationPivot(SerializedProperty property)
    {
        if (property.serializedObject.targetObject is PrefabSO instance)
        {
            return instance.Pivot;
        }

        return base.GetRotationPivot(property);
    }
}
