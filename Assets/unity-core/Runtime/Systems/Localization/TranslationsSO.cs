using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TranslationsSO), menuName = "Lavgine/UI/Translations")]
public class TranslationsSO : ScriptableObject
{
    public List<Translation> Translations = new();

    public string Format(string input)
    {
        var output = new StringBuilder();
        var start = 0;
        var end = 0;

        while (end < input.Length)
        {
            start = input.IndexOf("{{", end);

            if (start == -1)
            {
                output.Append(input.Substring(end));
                break;
            }

            output.Append(input.Substring(end, start - end));

            end = input.IndexOf("}}", start);

            if (end == -1)
            {
                output.Append(input.Substring(start));
                break;
            }

            var key = input.Substring(start + 2, end - start - 2);

            output.Append(this.FormatKey(key));

            end += 2;
        }

        return output.ToString();
    }

    public void OnValidate()
    {
        foreach (var translation in Translations)
        {
            translation.Normalize();
        }
    }

    private string FormatKey(string key)
    {
        return this.Translations.FirstOrDefault(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase))?.Value ?? key;
    }
}
