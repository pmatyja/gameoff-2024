using UnityEngine;

[CreateAssetMenu(fileName = nameof(VoiceLineSO), menuName = "Lavgine/Database.Audio/Voice Line")]
[ScriptableObject("AVL")]
public class VoiceLineSO : AudioResourceSO
{
    public override AudioLayer Layer { get; } = AudioLayer.VoiceLine;
}
