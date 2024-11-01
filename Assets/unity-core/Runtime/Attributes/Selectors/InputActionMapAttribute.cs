using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class InputActionMapAttribute : SelectorAttribute
{
    public override string GetItemGroup(object item)
    {
        return item?.ToString();
    }

    public override IEnumerable<object> GetItems(ReferenceInfo context, object parent)
    {
        var basePath = Path.Combine(Application.dataPath, "Resources");
        var files = Directory.EnumerateFiles(basePath, "*.inputactions", SearchOption.AllDirectories);

        var actions = new List<string>();

        foreach (var file in files)
        {
            var path = Path.GetRelativePath(basePath, file).Replace(".inputactions", string.Empty);
            var asset = Resources.Load<InputActionAsset>(path);

            foreach (var actionMap in asset.actionMaps)
            {
                actions.AddRange(actionMap.actions.Select(x => $"{asset.name}/{actionMap.name}/{x.name}"));
            }
        }

        return actions.Distinct();
    }
}
