using OCSFX.FMOD.AudioData;
using OCSFX.Generics;
using UnityEngine;

namespace OCSFX.FMOD
{
    public class OCSFXFMODSettings : SingletonScriptableObject<OCSFXFMODSettings>
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void Init() => UnityEditor.EditorApplication.delayCall += () => Get();
#endif
        
        [field: SerializeField] public VolumeSettingsAudioDataSO VolumeSettings { get; private set; }
        [field: SerializeField] public DebugAudioSO Debug { get; private set; }
        [field: SerializeField] public BanksAudioDataSO Banks { get; private set; }
        [field: SerializeField] public UiAudioDataSO UI { get; private set; }
        [field: SerializeField] public MusicAudioDataSO Music { get; private set; }
        [field: SerializeField] public AmbienceAudioDataSO Ambience { get; private set; }
        [field: SerializeField] public DialogueAudioDataSO Dialogue { get; private set; }
        [field: SerializeField] public SnapshotsAudioDataSO Snapshots { get; private set; }
        [field: SerializeField] public SurfacesAudioDataSO Surfaces { get; private set; }
    }
}