using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.World
{
    public class GameOff2024LevelSceneBase : MonoBehaviour
    {
        [field: SerializeField, Min(0)] public int LevelIndex { get; private set; }
        
        public static readonly HashSet<GameOff2024LevelSceneBase> Instances = new HashSet<GameOff2024LevelSceneBase>();

        public static event Action<GameOff2024LevelSceneBase> OnStart;
        
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInit()
        {
            Application.quitting += () => Instances.Clear();
        }
        
        private void Start()
        {
            if (Instances.Add(this))
            {
                OnStart?.Invoke(this);
            }
        }
    }
}