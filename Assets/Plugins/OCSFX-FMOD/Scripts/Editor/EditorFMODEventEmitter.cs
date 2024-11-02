using FMODUnity;
using UnityEditor;
using OCSFX.FMOD.Components;

namespace OCSFXEditor.FMOD
{
    /// <summary>
    /// Copy of StudioEventEmitter's Custom Editor so it will work on the OCSFX Custom version.
    /// </summary>
    
    [CustomEditor(typeof(FMODEventEmitter))]
    [CanEditMultipleObjects]
    public class EditorFMODEventEmitter: StudioEventEmitterEditor
    {
    }
}
