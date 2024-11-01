using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class AdvancedPopupItem : AdvancedDropdownItem
{
	public object Value { get; }

    public AdvancedPopupItem(int index, Texture2D icon, string name, object value) : base(name)
	{
		this.id = index;
		this.icon = icon;
        this.Value = value;
	}
}
