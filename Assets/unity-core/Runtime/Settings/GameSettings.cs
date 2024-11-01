using System;
using UnityEngine;

[Serializable]
public static class GameSettings
{
    public static void Set<T>(GameSettingsSubsystem subsystem, string id, T value)
    {
        PlayerPrefs.SetString($"{subsystem}.{id}", value.ToString());
        PlayerPrefs.Save();

        EventBus.Raise(null, new GameSettingsChangedEventParameters { Subsystem = subsystem, Id = id, Value = value });
    }

    public static T Get<T>(GameSettingsSubsystem subsystem, string id, T defaultValue, TryParseDelegate<string, T> parser)
    {
        id = $"{subsystem}.{id}";

        if (PlayerPrefs.HasKey(id))
        {
            if (parser.Invoke(PlayerPrefs.GetString(id), out var value))
            {
                return value;
            }
        }

        return defaultValue;
    }

    public static bool Get(GameSettingsSubsystem subsystem, string id, bool defaultValue)
    {
        return Get<bool>(subsystem, id, defaultValue, bool.TryParse);
    }

    public static int Get(GameSettingsSubsystem subsystem, string id, int defaultValue)
    {
        return Get<int>(subsystem, id, defaultValue, int.TryParse);
    }

    public static float Get(GameSettingsSubsystem subsystem, string id, float defaultValue)
    {
        return Get<float>(subsystem, id, defaultValue, float.TryParse);
    }

    public static bool TryGet<T>(GameSettingsSubsystem subsystem, string id, out T value, T defaultValue, TryParseDelegate<string, T> parser)
    {
        id = $"{subsystem}.{id}";

        if (PlayerPrefs.HasKey(id))
        {
            return parser.Invoke(PlayerPrefs.GetString(id), out value);
        }

        value = defaultValue;
        return false;
    }

    public static bool TryGet(GameSettingsSubsystem subsystem, string id, out bool value, bool defaultValue)
    {
        return TryGet<bool>(subsystem, id, out value, defaultValue, bool.TryParse);
    }

    public static bool TryGet(GameSettingsSubsystem subsystem, string id, out int value, int defaultValue)
    {
        return TryGet<int>(subsystem, id, out value, defaultValue, int.TryParse);
    }

    public static bool TryGet(GameSettingsSubsystem subsystem, string id, out float value, float defaultValue)
    {
        return TryGet<float>(subsystem, id, out value, defaultValue, float.TryParse);
    }

    public static bool TryGet(GameSettingsSubsystem subsystem, string id, out string value, string defaultValue)
    {
        id = $"{subsystem}.{id}";

        if (PlayerPrefs.HasKey(id))
        {
            value = PlayerPrefs.GetString(id);
            return true;
        }

        value = defaultValue;
        return false;
    }
}