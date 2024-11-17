using System.Collections.Generic;
using System.Linq;
using Runtime.Collectables;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ItemInventory))]
    public class ItemInventoryEditor : UnityEditor.Editor
    {
        private ItemInventory _itemInventory;

        private void OnEnable()
        {
            _itemInventory = (ItemInventory)target;
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Items", EditorStyles.boldLabel);

            var items = _itemInventory.GetItems();

            if (items.Count == 0)
            {
                EditorGUILayout.LabelField("No items in inventory", EditorStyles.helpBox);
            }
            else
            {
                var itemCounts = items.GroupBy(item => item.Data)
                                      .ToDictionary(group => group.Key, group => group.Count());

                EditorGUI.BeginDisabledGroup(true);
                foreach (var kvp in itemCounts)
                {
                    var data = kvp.Key;
                    var count = kvp.Value;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(data, typeof(CollectableData), false);
                    EditorGUILayout.LabelField(!data.IsUnique ? $"Count: {count}" : "Unique");
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}