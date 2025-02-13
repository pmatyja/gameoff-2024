using UnityEngine;

namespace OCSFX.EZFMOD.ScriptableObjects
{
    public abstract class EZFMODAudioDataSO : ScriptableObject
    {
        protected const string _CREATE_ASSET_MENU_BASE = "OCSFX/FMOD/Audio Data/";

        [SerializeField] protected bool _showDebug;
    }
}
