using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameOff2024.Editor
{
    public class EditorPrefabReplacerWindow : EditorWindow
    {
        [SerializeField] private GameObject prefab;

        [MenuItem("Tools/Replace With Prefab")]
        static void CreateReplaceWithPrefab()
        {
            EditorWindow.GetWindow<EditorPrefabReplacerWindow>();
        }

        private void OnGUI()
        {
            prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

            if (GUILayout.Button("Replace"))
            {
                ReplaceSelectionWithPrefab();
            }

            GUI.enabled = false;
            EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
        }

        private void ReplaceSelectionWithPrefab()
        {
            var selection = Selection.gameObjects;

            for (var i = selection.Length - 1; i >= 0; --i)
            {
                var selected = selection[i];
                var newObject = GetNewObject();

                if (!newObject)
                {
                    Debug.LogError("Error instantiating prefab");
                    break;
                }

                Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
            
                CopyProperties(newObject, selected);

                Undo.DestroyObjectImmediate(selected);
            }
        }

        private GameObject GetNewObject()
        {
            var prefabType = PrefabUtility.GetPrefabAssetType(prefab);
            GameObject newObject;

            if (prefabType != PrefabAssetType.MissingAsset && prefabType != PrefabAssetType.NotAPrefab)
            {
                newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            }
            else
            {
                newObject = Instantiate(prefab);
                newObject.name = prefab.name;
            }

            return newObject;
        }

        private static void CopyProperties(GameObject newObject, GameObject selected)
        {
            var newObjectComponents = newObject.GetComponents<Component>().ToHashSet();
            var oldObjectComponents = selected.GetComponents<Component>().ToHashSet();
                
            foreach (var oldComponent in oldObjectComponents)
            {
                foreach (var newComponent in newObjectComponents)
                {
                    var type = newComponent.GetType();
                    if (type == typeof(Transform)) continue;
                        
                    if (type == oldComponent.GetType())
                    {
                        CopyComponentValues(oldComponent, newComponent);
                    }
                }
            }
                
            newObject.transform.parent = selected.transform.parent;
            newObject.transform.localPosition = selected.transform.localPosition;
            newObject.transform.localRotation = selected.transform.localRotation;
            newObject.transform.localScale = selected.transform.localScale;
            newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
                
            newObject.SetActive(selected.activeSelf);
        }

        private static void CopyComponentValues(Component sourceComponent, Component targetComponent)
        {
            var type = sourceComponent.GetType();
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var fields = type.GetFields(flags);
            var properties = type.GetProperties(flags);

            foreach (var field in fields)
            {
                field.SetValue(targetComponent, field.GetValue(sourceComponent));
            }
        
            // Special cases: Skip properties that should not be modified directly
            var skipPropertyNames = new string[]
            {
                "alphaHitTestMinimumThreshold",
                "eventAlphaThreshold"
            };

            foreach (var property in properties)
            {
                if (!property.CanWrite) continue;

                if (skipPropertyNames.Contains(property.Name)) continue;
                
                property.SetValue(targetComponent, property.GetValue(sourceComponent));
            }
        }
    }
}