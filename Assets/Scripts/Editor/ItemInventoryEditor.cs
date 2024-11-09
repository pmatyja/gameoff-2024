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

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Item {i}", GUILayout.Width(100));
                items[i] = (CollectableData)EditorGUILayout.ObjectField(items[i], typeof(CollectableData), false);
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}