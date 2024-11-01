using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class AdvancedPopupGroup : AdvancedDropdownItem
{
    public AdvancedPopupGroup(int index, string name) : base(name)
    {
        this.id = index;
        this.icon = EditorOnly.GetIcon("d_Project");
    }

    public static AdvancedDropdownItem GetOrAdd(AdvancedDropdownItem root, string path, char separator = '/')
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return root;
        }

        if (root.name == path)
        {
            return root;
        }

        var separatorIndex = path.IndexOf(separator);
        if (separatorIndex == -1)
        {
            return root;
        }

        var prerfix = ObjectNames.NicifyVariableName( path.Substring(0, separatorIndex) );

        var child = root.children.FirstOrDefault(x => x.name == prerfix);
        if (child == null)
        {
            child = new AdvancedPopupGroup(root.children.Count(), prerfix);
            root.AddChild(child);
        }

        return GetOrAdd(child, path.Substring(separatorIndex + 1), separator);
    }
}
