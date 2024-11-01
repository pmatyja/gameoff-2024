using System.Collections;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

public class PrefabWindowEditor : EditorWindow
{
    private static int TileSize = 5;
    private static int ItemsLimit = int.MaxValue;
    private static EditorCoroutine coroutine;

    [MenuItem("Lavgine/Prefab Repository")]
    private static void Init()
    {
        var window = GetWindow(typeof(PrefabWindowEditor)) as PrefabWindowEditor;
        window.Show();
    }

    private void OnGUI()
    {
        TileSize = EditorGUILayout.IntSlider(new GUIContent("Tile size"), TileSize, 1, 32);
        ItemsLimit = EditorGUILayout.IntSlider(new GUIContent("Item limit (for testing only)"), ItemsLimit, 1, int.MaxValue);

        if (GUILayout.Button("Auto generate all prefabs"))
        {
            if (coroutine != null)
            {
                Debug.Log("Generation already working");
                return;
            }

            coroutine = EditorCoroutineUtility.StartCoroutine(Generate(), this);
        }
    }

    private static IEnumerator Generate()
    {
        var progressId = Progress.Start("Prefabs generation");
        var guids = AssetDatabase.FindAssets("t:" + typeof(GameObject).Name, new[]
        {
            ""
        });

        try
        {
            var count = Mathf.Min(ItemsLimit, guids.Length);

            for ( var i = 0; i < count; )
            {
                if (Progress.GetStatus(progressId) == Progress.Status.Paused)
                {
                    yield return null;
                }

                if (Progress.GetStatus(progressId) != Progress.Status.Running)
                {
                    break;
                }

                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var instance = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (instance == null)
                {
                    Debug.LogWarning($"Prefab asset is null: {path}");
                    continue;
                }

                var output = $"Assets/Models/Blueprints/Auto/{instance.name}.asset";
                var isNew = false;

                var prefab = AssetDatabase.LoadAssetAtPath<PrefabSO>(output);
                if (prefab == null)
                {
                    prefab = ScriptableObject.CreateInstance<PrefabSO>();
                    isNew = true;
                }

                if (prefab == null)
                {
                    Debug.LogWarning($"Prefab is null: {path}");
                    continue;
                }

                prefab.Model = instance;
                prefab.OnUpdate();

                if (isNew)
                {
                    AssetDatabase.CreateAsset(prefab, output);
                }
                else
                {
                    AssetDatabase.SaveAssetIfDirty(prefab);
                }

                Progress.Report(progressId, i, guids.Count());
                ++i;

                yield return null;
            }
        }
        finally
        {
            Progress.Remove(progressId);
            coroutine = null;
        }
    }
}