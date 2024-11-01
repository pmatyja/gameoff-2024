using System;

[Serializable]
public class Biome
{
    public TemperatureProperties Temperature = new();
    public EvaporationProperties Evaporation = new();
    public WindProperties Wind = new();
}