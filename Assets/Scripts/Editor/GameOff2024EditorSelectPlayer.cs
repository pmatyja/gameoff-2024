using Runtime;
using UnityEditor;

namespace Editor
{
    public static class GameOff2024EditorSelectPlayer
    {
        [MenuItem(GameOff2024Statics.MENU_ROOT + "Select Player")]
        public static void SelectPlayer()
        {
            // Select the player gameobject in the hierarchy
            Selection.activeObject = GameOff2024Statics.GetPlayerCharacter();
        }
    }
}