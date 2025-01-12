using Runtime;
using Runtime.Utility;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class GameOff2024EditorMenuItems
    {
        [MenuItem(GameOff2024Statics.MENU_ROOT + "Select Player")]
        public static void SelectPlayer()
        {
            // Select the player gameobject in the hierarchy
            Selection.activeObject = GameOff2024Statics.GetPlayerCharacter();
        }
        
        [MenuItem(GameOff2024Statics.MENU_ROOT + "Assign Unique IDs in Scene")]
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
        
        [MenuItem(GameOff2024Statics.MENU_ROOT + "Load Scene", false, 0)]
        public static void ShowLoadSceneWindow()
        {
            EditorSceneLoadWindow.ShowWindow();
        }
    }
}