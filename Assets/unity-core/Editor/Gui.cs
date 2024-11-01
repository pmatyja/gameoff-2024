using UnityEditor;

public static class Gui
{
    public const float IndentWidth = 16.0f;
    public const float ColumnSpacing = 4.0f;

    public static readonly float RowSpacing = EditorGUIUtility.standardVerticalSpacing;
    public static readonly float LineHeight = EditorGUIUtility.singleLineHeight;
    public static readonly float NextLine = Gui.LineHeight + Gui.RowSpacing;
}
