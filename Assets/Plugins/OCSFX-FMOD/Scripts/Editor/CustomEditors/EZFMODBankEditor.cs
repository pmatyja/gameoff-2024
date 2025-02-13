using OCSFX.EZFMOD.Types;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OCSFX.EZFMODEditor.CustomEditors
{
    [CustomEditor(typeof(EZFMODBank))]
    public class EZFMODBankEditor : EZFMODAssetEditor<EZFMODBank>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_asset && _asset.IsMasterBank)
            {
                EditorGUILayout.Space();
                GUI.color = Color.cyan;
                EditorGUILayout.HelpBox("MASTER BANK", MessageType.Info);
                GUI.color = default;
            }
        }
    }
}