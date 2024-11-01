using System.Runtime.CompilerServices;

public static class HashCode
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Combine(int left, int right)
    {
        unchecked
        {
            // RyuJIT optimizes this to use the ROL instruction
            // Related GitHub pull request: dotnet/coreclr#1830
            var rol5 = ((uint)left << 5) | ((uint)left >> 27);
            return ((int)rol5 + left) ^ right;
        }
    }
}