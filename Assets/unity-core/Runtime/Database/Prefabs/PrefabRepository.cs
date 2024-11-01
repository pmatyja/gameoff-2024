using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(PrefabRepository), menuName = "Lavgine/Database.Prefab/Prefab Repository")]
public class PrefabRepository : ScriptableObject
{
    public List<PrefabSO> Items;

    public PrefabSO Find(string id)
    {
        return this.Items.FirstOrDefault(x => x.Id == id);
    }
}