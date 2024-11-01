using UnityEngine;

[CreateAssetMenu(fileName = nameof(SoundEffectSO), menuName = "Lavgine/Database.Audio/Sound Effect")]
[ScriptableObject("ASE")]
public class SoundEffectSO : AudioResourceSO
{
    public override AudioLayer Layer { get; } = AudioLayer.SoundEffect;
}
