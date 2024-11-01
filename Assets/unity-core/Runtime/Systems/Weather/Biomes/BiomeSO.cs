using UnityEngine;

[CreateAssetMenu(fileName = nameof(BiomeSO), menuName = "Lavgine/Database.Systems/Weather Biome")]
public class BiomeSO : ScriptableObject
{
    public AmbientSO AmbientDay;
    public AmbientSO AmbientNight;
    public TemperatureProperties Temperature = new();
    public EvaporationProperties Evaporation = new();
    public WindProperties Wind = new();
}