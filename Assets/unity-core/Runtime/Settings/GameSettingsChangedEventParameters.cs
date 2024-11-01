using System;

[Serializable]
public struct GameSettingsChangedEventParameters
{
    public GameSettingsSubsystem Subsystem;
    public string Id;
    public dynamic Value;
}
