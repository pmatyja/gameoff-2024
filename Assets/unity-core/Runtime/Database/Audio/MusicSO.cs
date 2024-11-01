using UnityEngine;

[CreateAssetMenu(fileName = nameof(MusicSO), menuName = "Lavgine/Database.Audio/Music")]
[ScriptableObject("AMU")]
public class MusicSO : AudioResourceSO
{
    public override AudioLayer Layer { get; } = AudioLayer.Music;
}
