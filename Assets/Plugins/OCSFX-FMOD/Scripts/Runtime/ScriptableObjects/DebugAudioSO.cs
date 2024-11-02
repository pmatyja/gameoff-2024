using FMODUnity;
using UnityEngine;

namespace OCSFX.FMOD.AudioData
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Debug", fileName = nameof(DebugAudioSO))]
    public class DebugAudioSO : AudioDataSO
    {
        // References to mix to appropriate loudness level
        // Create events in FMOD as infinite loops with these sources and loudness targets
        [SerializeField] private EventReference _sineWaveMinus24LUFS;
        [SerializeField] private EventReference _pinkNoiseMinus24LUFS;
    }   
}
