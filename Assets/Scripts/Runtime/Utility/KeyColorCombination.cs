using Runtime.Collectables;
using UnityEngine;

namespace Runtime.Utility
{
    [CreateAssetMenu(menuName = GameOff2024Statics.MENU_ROOT + nameof(KeyColorCombination))]
    public class KeyColorCombination : ScriptableObject
    {
        public CollectableData[] Combination;
            
        [ColorUsage(true)] public Color CombinedBaseColor = Color.white;
        [ColorUsage(true, true)] public Color CombinedEmissionColor = Color.white;
    }
}