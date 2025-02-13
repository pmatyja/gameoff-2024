using FMODUnity;
using OCSFX.EZFMOD.Components;
using UnityEditor;

namespace OCSFX.EZFMODEditor.CustomEditors
{
    /// <summary>
    /// Copy of StudioEventEmitter's Custom Editor so it will work on the OCSFX Custom version.
    /// </summary>
    
    [CustomEditor(typeof(EZFMODEventEmitter))]
    [CanEditMultipleObjects]
    public class EZFMODEventEmitterEditor: StudioEventEmitterEditor
    {
    }
}
