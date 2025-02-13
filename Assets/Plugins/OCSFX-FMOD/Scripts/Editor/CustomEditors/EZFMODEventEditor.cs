using OCSFX.EZFMOD.Types;
using UnityEditor;

namespace OCSFX.EZFMODEditor.CustomEditors
{
    [CustomEditor(typeof(EZFMODEvent))]
    public class EZFMODEventEditor : EZFMODAssetEditor<EZFMODEvent>
    {
        private static bool _banksExpanded = true;
        private static bool _parametersExpanded = true;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!_asset) return;
            
            DrawIs3D();
            DrawIsOneShot();
            DrawMinMaxDistance();
            DrawLength();
            DrawBanks();
            DrawParameters();
        }
        
        private void DrawBanks()
        {
            // Draw banks in a collapsable format with a label "Banks"
            var banks = _asset.Banks;
            
            if (banks == null || banks.Length == 0)
            {
                // Draw a warning message if there are no banks
                _banksExpanded = EditorGUILayout.Foldout(_banksExpanded, "Banks", true);
                if (_banksExpanded)
                {
                    EditorGUILayout.HelpBox($"Event ({_asset.Name}) is not assigned to any Banks.", MessageType.Warning);
                }
            }
            else
            {
                _banksExpanded = DrawObjectArray(_banksExpanded, "Banks", banks);   
            }
        }
        
        private void DrawParameters()
        {
            // Draw parameters in a collapsable format with a label "Parameters"
            var parameters = _asset.Parameters;
            _parametersExpanded = DrawObjectArray(_parametersExpanded, "Parameters", parameters);
        }
        
        private void DrawMinMaxDistance()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.FloatField("Min Distance", _asset.MinDistance);
            EditorGUILayout.FloatField("Max Distance", _asset.MaxDistance);
            EditorGUI.EndDisabledGroup();
        }
        
        private void DrawLength()
        {
            var eventAsset = (EZFMODEvent)serializedObject.targetObject;
            if (!eventAsset)
            {
                return;
            }
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.FloatField("Length", eventAsset.Length);
            EditorGUI.EndDisabledGroup();
        }
        
        private void DrawIs3D()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Is 3D", _asset.Is3D);
            EditorGUI.EndDisabledGroup();
        }
        
        private void DrawIsOneShot()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Is One Shot", _asset.IsOneShot);
            EditorGUI.EndDisabledGroup();
        }
    }
}