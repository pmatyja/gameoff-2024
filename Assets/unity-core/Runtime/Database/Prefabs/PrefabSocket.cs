using System;

[Serializable]
public struct PrefabSocket
{
    public PrefabSocketGroup SkipMatched;
    public PrefabSocketSO Left;
    public PrefabSocketSO Middle;
    public PrefabSocketSO Right;

    public static bool Match(PrefabSocket center, PrefabSocket adjacent, bool mirror = false)
    {
        if (((ulong)center.SkipMatched & (ulong)adjacent.SkipMatched) > 0)
        {
            return false;
        }

        if (center.Left == null || center.Middle == null || center.Right == null || adjacent.Left == null || adjacent.Middle == null || adjacent.Right == null)
        {
            return false;
        }

        if (mirror)
        {
            return
                center.Middle == adjacent.Middle &&
                center.Left  == adjacent.Right && 
                center.Right == adjacent.Left;
        }

        return 
            center.Middle == adjacent.Middle &&
            center.Left  == adjacent.Left && 
            center.Right == adjacent.Right;
    }
}