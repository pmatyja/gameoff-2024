using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    public class EditorRandomRotatorWindow : EditorWindow
    {
        [SerializeField] private Transform[] _targets;
        [SerializeField] private Vector3 _rotationSteps;
        
        [MenuItem("Tools/Random Rotator")]
        static void CreateReplaceWithPrefab()
        {
            EditorWindow.GetWindow<EditorRandomRotatorWindow>();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Random Rotator", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Set the size of the rotation steps", MessageType.Info);
            _rotationSteps = EditorGUILayout.Vector3Field("RotationSteps", _rotationSteps);
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Select the objects you want to rotate randomly", MessageType.Info);
            
            // Set _targets to all the currently selected objects
            _targets = Selection.transforms;
            
            // Display the number of selected objects
            EditorGUILayout.LabelField("Selected objects: " + _targets.Length);
            
            if (GUILayout.Button("Execute"))
            {
                _targets = Selection.transforms;
                
                Undo.RecordObjects(_targets, "Random Rotation");
                
                foreach (var target in _targets)
                {
                    // For each axis, get a random value from 0-360,
                    // but it must be multiple of the step value of that axis
                    
                    var x = Mathf.Round(UnityEngine.Random.Range(0, 360) / _rotationSteps.x) * _rotationSteps.x;
                    var y = Mathf.Round(UnityEngine.Random.Range(0, 360) / _rotationSteps.y) * _rotationSteps.y;
                    var z = Mathf.Round(UnityEngine.Random.Range(0, 360) / _rotationSteps.z) * _rotationSteps.z;
                    
                    // Make sure to rotate around the object's center rather than its pivot
                    var centerPosition = target.GetComponent<Renderer>().bounds.center;
                    target.RotateAround(centerPosition, Vector3.right, x);
                    target.RotateAround(centerPosition, Vector3.up, y);
                    target.RotateAround(centerPosition, Vector3.forward, z);
                    
                    EditorUtility.SetDirty(target);
                }
            }
        }
    }
}