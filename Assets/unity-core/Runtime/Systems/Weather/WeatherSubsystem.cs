using System;

[Flags]
public enum WeatherSubsystem
{
    None = 0,
    Time = 1 << 0,
    Temperature = 1 << 1,
    Humidity = 1 << 2,
    Evaporation = 1 << 3,
    WindIntensity = 1 << 4,
    WindDirection = 1 << 5
}