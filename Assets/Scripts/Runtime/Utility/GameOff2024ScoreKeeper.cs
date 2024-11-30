using System;
using Runtime.Collectables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Utility
{
    public static class GameOff2024ScoreKeeper
    {
        public static event Action<GameOff2024Score> OnGameEndedEvent;
        
        public static int OptionalItemsCollected { get; private set; }
        public static float GameplayTime => GetTimeElapsed();
        
        public static float GetTimeElapsed()
        {
            if (_timeAtGameStart == 0) return 0;
            if (_timeAtGameEnd == 0) return Time.time - _timeAtGameStart;
            return _timeAtGameEnd - _timeAtGameStart;
        }
        
        private static float _timeAtGameStart;
        private static float _timeAtGameEnd;

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            ResetValues();

            SceneManager.sceneLoaded += OnSceneLoaded;
            
            Application.quitting += Deinitialize;
        }

        private static void Deinitialize()
        {
            ResetValues();
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == GameOff2024GameSettings.Get().StartGameSceneName)
            {
                OnNewGameStarted();
            }
            else if (scene.name == GameOff2024GameSettings.Get().EndingSceneName)
            {
                OnGameEnded();
            }
        }

        private static void ResetValues()
        {
            _timeAtGameStart = 0;
            _timeAtGameEnd = 0;
        }
        
        private static void OnNewGameStarted()
        {
            ResetValues();
            
            _timeAtGameStart = Time.time;
            
            ItemInventory.OnOptionalItemsCollectedChanged += OnOptionalItemsCollectedChanged;
            
            Debug.Log("Game started");
        }

        private static void OnOptionalItemsCollectedChanged(int optionalItemCount)
        {
            OptionalItemsCollected = optionalItemCount;
        }

        private static void OnGameEnded()
        {
            _timeAtGameEnd = Time.time;

            var score = new GameOff2024Score(OptionalItemsCollected, GameplayTime);
            
            OnGameEndedEvent?.Invoke(score);
            
            Debug.Log($"Game ended with score: {score.OptionalCollectablesCollected} collectables collected in {score.CompletionTime} seconds");
        }
    }
    
    public struct GameOff2024Score
    {
        public int OptionalCollectablesCollected;
        public float CompletionTime;
        
        public GameOff2024Score(int optionalCollectablesCollected, float completionTime)
        {
            OptionalCollectablesCollected = optionalCollectablesCollected;
            CompletionTime = completionTime;
        }
    }
}