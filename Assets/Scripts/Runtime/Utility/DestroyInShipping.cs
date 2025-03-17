using System;
using UnityEngine;

public class DestroyInShipping : MonoBehaviour
{
    private void Awake()
    {
        // Destroy this object if not in the editor and not a debug build
#if !UNITY_EDITOR
        if (!Debug.isDebugBuild)
        {
            Destroy(gameObject);
        }
#endif
    }
}
