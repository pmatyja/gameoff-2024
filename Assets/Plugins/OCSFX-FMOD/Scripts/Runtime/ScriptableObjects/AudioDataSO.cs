using UnityEngine;

namespace OCSFX.FMOD.AudioData
{
    public abstract class AudioDataSO : ScriptableObject
    {
        protected const string _CREATE_ASSET_MENU_BASE = "OCSFX/FMOD/Audio Data/";

        [SerializeField] protected bool _showDebug;
    }
}
