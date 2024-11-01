using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabGroupSO))]
public class PrefabGroupSOEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        var component = this.target as PrefabGroupSO;

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Recreate group pairs"))
            {
                component.Rebuild();
            }

            if (GUILayout.Button("Update group pairs"))
            {
                component.Update();
            }
        }
        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}
