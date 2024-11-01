using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(PrefabSO))]
public class PrefabSOEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        var component = this.target as PrefabSO;

        if (component.Pairs == null)
        {
            component.Pairs = new List<PrefabPair>();
        }

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Clear pairs"))
            {
                component.Pairs.Clear();
            }

            if (GUILayout.Button("Update"))
            {
                component.OnUpdate();
            }

            if (GUILayout.Button("Recreate pairs"))
            {
                component.Pairs.Clear();
                component.UpdatePrefabPairs();
            }

            if (GUILayout.Button("Update pairs"))
            {
                component.UpdatePrefabPairs();
            }
        }
        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}
