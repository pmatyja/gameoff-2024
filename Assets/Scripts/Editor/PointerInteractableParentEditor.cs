using GameOff2024;
using GameOff2024.Interactions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameOff2024.Editor
{
    [CustomEditor(typeof(PointerInteractableParent))]
    public class PointerInteractableParentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(
                "This script adds PointerInteractable component to every child of this GameObject with a Collider.\n" +
                "This allows this parent to then subscribe to their OnMouseDown.", EditorStyles.helpBox);
            
            base.OnInspectorGUI();
        }
    }
}