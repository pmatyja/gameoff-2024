using FMOD;
using OCSFX.EZFMOD.Attributes;
using UnityEngine;

namespace OCSFX.EZFMOD.Types
{
    public abstract class EZFMODAsset : ScriptableObject
    {
        [field: SerializeField, ReadOnly] public string Name { get; protected set; }

        [field: SerializeField, ReadOnly] public string StudioPath { get; protected set; }

        [field: SerializeField, ReadOnly] public GUID GUID { get; protected set; }
    }
}