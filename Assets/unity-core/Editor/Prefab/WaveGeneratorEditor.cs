using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaveGenerator))]
public class WaveGeneratorEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var component = this.target as WaveGenerator;

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Clear"))
            {
                component.Clear();
            }

            if (GUILayout.Button("Generate"))
            {
                component.Generate();
            }

            if (GUILayout.Button("Random"))
            {
                Rng.NextSeed(ref component.Seed);
                component.Generate();
            }
        }
        GUILayout.EndHorizontal();
    }
}