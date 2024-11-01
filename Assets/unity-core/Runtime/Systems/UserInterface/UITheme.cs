using System;

public static class UITheme
{
    public const float TransitionDuration = 0.25f;
    public const float CharactersReadingPacePerSeconds = 50;
    public const float MinTextDisplayDuration = 2.0f;

    public static float GetTextDisplayTime(string text, in float minTextDisplayDuration = UITheme.MinTextDisplayDuration)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0.0f;
        }

        return Math.Max(MinTextDisplayDuration, text.Length / CharactersReadingPacePerSeconds);
    }
}
