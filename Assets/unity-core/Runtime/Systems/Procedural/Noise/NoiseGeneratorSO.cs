using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(NoiseGeneratorSO), menuName = "Lavgine/Database.Procedural/Noise Generator")]
public class NoiseGeneratorSO : ScriptableObject
{
    [SerializeField]
    [ProceduralTexture(Size = 512)]
    private Texture2D previewTexture;

    [SerializeField]
    private Gradient previewGradient = new()
    {
        colorKeys = new[]
        {
            new GradientColorKey(Color.black, 0.0f),
            new GradientColorKey(Color.white, 1.0f)
        }
    };

    [Header("General")]

    [Range(0, 200000)]
    public ulong InlineSeed = 1;

    public SeedSourceSO ExternalSeed;

    [Range(0.0f, 1.0f)]
    public float Scale = 1.0f;

    [Range(0.0001f, 1.0f)]
    public float FrequnecyX = 0.02f;

    [Range(0.0001f, 1.0f)]
    public float FrequnecyY = 0.02f;

    [Range(0, 128)]
    public int TerraceSteps;

    [Header("Curve")]
    public bool EnableCurve;
    public AnimationCurve Curve = new()
    {
        keys = new[]
        {
            new Keyframe(0.0f, 0.0f, 1.0f, 1.0f),
            new Keyframe(1.0f, 1.0f, 1.0f, 1.0f)
        }
    };

    [Header("Fractional Brownian Motion")]

    [Range(0.0f, 4.0f)]
    public float Lacunarity = 2.0f;

    [SerializeReference]
    [TypeInstanceSelector(Label = LabelState.Default)]
    public OctaveGenerator OctaveGenerator = new PersistenceOctaveGenerator();

    [SerializeReference]
    [TypeInstanceSelector]
    public List<NoiseValueProcessor> PostProcessors = new();

    public float Generate(float x, float y = 1.0f)
    {
        var seed = this.ExternalSeed?.Value ?? this.InlineSeed;

        var nx = x * this.FrequnecyX + seed;
        var ny = y * this.FrequnecyY + seed;

        var value = this.OctaveGenerator?.Generate(this.Lacunarity, nx, ny) ?? Mathf.PerlinNoise(nx, ny);

        foreach (var processor in this.PostProcessors)
        {
            if (processor != null && processor.Enabled)
            {
                value = processor.Process(value, x, y);
            }
        }

        if (this.TerraceSteps > 0)
        {
            value = Mathf.Round( value * this.TerraceSteps ) / this.TerraceSteps;
        }

        if (this.EnableCurve)
        {
            value = this.Curve.Evaluate(value);
        }

        return Mathf.Clamp01(value * this.Scale);
    }

    public void OnValidate()
    {
        PreviewTexture2D.Generate(out this.previewTexture, (x, y) => this.previewGradient.Evaluate(this.Generate(x, y)), 128, FilterMode.Point);
    }
}
