using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderBasic : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
