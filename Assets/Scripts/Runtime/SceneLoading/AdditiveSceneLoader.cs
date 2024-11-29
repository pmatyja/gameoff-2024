using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneLoader : MonoBehaviour
{
    [field: SerializeField] public List<string> AdditiveSceneNames { get; private set; } = new List<string>();

    private void Awake()
    {
        LoadAll();
    }

    public void LoadAll()
    {
        foreach (var sceneName in AdditiveSceneNames)
        {
            Load(sceneName);
        }
    }
    
    private void Load(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);   
    }
    
    public void UnloadAll()
    {
        foreach (var sceneName in AdditiveSceneNames)
        {
            Unload(sceneName);
        }
    }

    private void Unload(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
}
