using System;
using System.Collections;
using UnityEngine;

namespace Runtime.World
{
    [RequireComponent(typeof(BoxCollider))]
    public class GameOff2024LevelSceneBaseTrigger : MonoBehaviour
    {
        [field: SerializeField, Min(0)] public int LevelIndex { get; private set; }
        
        [field: SerializeField, Readonly] 
        public GameOff2024LevelSceneBase LevelSceneBase { get; private set; }
        
        private void OnEnable()
        {
            GameOff2024LevelSceneBase.OnStart += OnLevelBaseStart;
        }

        private void OnDisable()
        {
            GameOff2024LevelSceneBase.OnStart -= OnLevelBaseStart;
        }

        private void OnLevelBaseStart(GameOff2024LevelSceneBase levelSceneBase)
        {
            if (levelSceneBase.LevelIndex != this.LevelIndex) return;
            LevelSceneBase = levelSceneBase;
            StartCoroutine(Co_OnEndOfFrame());
        }
        
        private IEnumerator Co_OnEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            LevelSceneBase.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(GameOff2024Statics.GetPlayerTag())) return;
            
            if (!LevelSceneBase) return;
            LevelSceneBase.gameObject.SetActive(true);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(GameOff2024Statics.GetPlayerTag())) return;
            
            if (!LevelSceneBase) return;
            LevelSceneBase.gameObject.SetActive(false);
        }
        
        private void OnValidate()
        {
            var col = GetComponent<BoxCollider>();
            col.isTrigger = true;
        }
    }
}