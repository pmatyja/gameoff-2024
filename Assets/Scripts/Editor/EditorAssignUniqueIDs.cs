using Runtime;
using Runtime.Utility;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class EditorAssignUniqueIDs
    {
        [MenuItem(GameOff2024Statics.MENU_ROOT + "/ Assign Unique IDs in Scene", false, 0)]
        public static void AssignUniqueIDsInScene()
        {
            var gameOff2024UniqueIDs = 
                Object.FindObjectsByType<GameOff2024UniqueID>(
                    FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            
            foreach (var uniqueID in gameOff2024UniqueIDs)
            {
                uniqueID.GenerateID();
            }
        }
    }
}