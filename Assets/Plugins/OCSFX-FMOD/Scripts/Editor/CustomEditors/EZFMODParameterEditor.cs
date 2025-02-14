using OCSFX.EZFMOD.Types;
using UnityEditor;

namespace OCSFX.EZFMODEditor.CustomEditors
{
    [CustomEditor(typeof(EZFMODParameter))]
    public class EZFMODParameterEditor : EZFMODAssetEditor<EZFMODParameter>
    {
        private static bool _idExpanded;
        private static bool _labelsExpanded = true;
        
        private readonly string[] _idAsStringArray = new string[2];
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawParameterID();
            DrawParameterType();
            DrawValues();
            DrawLabels();
        }
        
        private void DrawParameterID()
        {
            _idAsStringArray[0] = _asset.ID.data1.ToString();
            _idAsStringArray[1] = _asset.ID.data2.ToString();
            
            _idExpanded = DrawStringArray(_idExpanded, "ID", _idAsStringArray);
        }
        
        private void DrawParameterType()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup("Type", _asset.Type);
            EditorGUI.EndDisabledGroup();
        }
        
        private void DrawValues()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.FloatField("Min", _asset.Min);
            EditorGUILayout.FloatField("Max", _asset.Max);
            EditorGUILayout.FloatField("Default", _asset.Default);
            EditorGUI.EndDisabledGroup();
        }

        private void DrawLabels()
        {
            if (_asset.Type != ParameterType.Labeled) return;
            
            _labelsExpanded = DrawStringArray(_labelsExpanded, "Labels", _asset.Labels);
        }
    }
}