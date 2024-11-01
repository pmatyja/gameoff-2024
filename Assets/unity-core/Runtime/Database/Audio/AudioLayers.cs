using System;

[Flags]
public enum AudioLayers
{
    SoundEffect = 1 << 0,
    Music = 1 << 1,
    UI = 1 << 2,
    Ambient = 1 << 3,
    VoiceLine = 1 << 4,
    All = 0x7FFFFFFF
}