using System.Collections.Generic;
using UnityEngine;

public interface IWaveFunctionCollapseTile
{
    Vector3Int Coordinates { get; }
    bool Resolved { get; set; }
    PrefabPair Final { get; set; }
    IEnumerable<PrefabPair> Pairs { get; set; }
}
