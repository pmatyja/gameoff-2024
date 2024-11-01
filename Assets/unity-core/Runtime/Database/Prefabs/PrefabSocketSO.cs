using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(PrefabSocketSO), menuName = "Lavgine/Database.Prefab/Prefab Socket")]
public class PrefabSocketSO : ScriptableObject
{
    [Readonly]
    public string Id = Guid.NewGuid().ToString();

    public List<PrefabSO> Examples;
}
