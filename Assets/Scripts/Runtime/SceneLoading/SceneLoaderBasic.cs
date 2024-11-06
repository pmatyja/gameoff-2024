using UnityEngine;

namespace GameOff2024.SceneLoading
{
    public class SceneLoaderBasic : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}
