using System;
using UnityEngine;

public static class PreviewTexture2D
{
    public static void Generate(out Texture2D preview, Func<float, float, Color> onPixel, int size = 256, FilterMode filtering = FilterMode.Bilinear)
    {
        preview = new Texture2D(size, size, TextureFormat.RGB24, false);
        preview.name = $"(Generated Texture: {size}x{size}:{filtering})";
        preview.filterMode = filtering;

        for (var y = 0; y < size; ++y)
        {
            for (var x = 0; x < size; ++x)
            {
                preview.SetPixel(x, y, onPixel.Invoke(x, y));
            }
        }

        preview.Apply();
    }
}
