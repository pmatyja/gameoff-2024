using UnityEngine;

[CreateAssetMenu(fileName = nameof(AmbientSO), menuName = "Lavgine/Database.Audio/Ambient")]
[ScriptableObject("AAM")]
public class AmbientSO : AudioResourceSO
{
    public override AudioLayer Layer { get; } = AudioLayer.Ambient;
}