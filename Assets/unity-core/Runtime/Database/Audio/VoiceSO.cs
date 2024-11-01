using UnityEngine;

[CreateAssetMenu(fileName = nameof(VoiceSO), menuName = "Lavgine/Database.Audio/Voice")]
[ScriptableObject("AVO")]
public class VoiceSO : AudioResourceSO
{
    public override AudioLayer Layer { get; } = AudioLayer.VoiceLine;
}
