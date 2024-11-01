using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public sealed class TranslationAttribute : SelectorAttribute
{
    public TranslationAttribute()
    {
        this.AllowManualEdit = true;
    }

    public override string GetItemGroup(object item)
    {
        return string.Empty;
    }

    public override IEnumerable<object> GetItems(ReferenceInfo context, object parent)
    {
        var basePath = Path.Combine(Application.dataPath, "Resources");

        var results = new List<string>();
        var files = new List<TranslationsSO>();

        EditorOnly.LoadAsset<TranslationsSO>("Assets/Resources/", "*.asset", files, SearchOption.AllDirectories);

        foreach (var file in files)
        {
            results.AddRange(file.Translations.Select(x => x.Name));
        }

        return results;
    }

    public override object GetValue(object item)
    {
        var value = item?.ToString();

        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return "{{" + value + "}}";
    }
}
