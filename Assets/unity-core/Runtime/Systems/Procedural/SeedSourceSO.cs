using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SeedSourceSO), menuName = "Lavgine/Database.Procedural/Seed Source")]
public class SeedSourceSO : ScriptableObject
{
    [SerializeField]
    private ulong seed = 1;
    public ulong Value { get => this.seed; set => this.seed = Math.Max(value, 1); }
}