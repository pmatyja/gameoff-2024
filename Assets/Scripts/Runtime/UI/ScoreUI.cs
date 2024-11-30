using Runtime.Utility;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class ScoreUI : OCSFX.Generics.Singleton<ScoreUI>
    {
        [SerializeField] private TMP_Text _completionPercentageText;
        [SerializeField] private TMP_Text _gameplayTimeText;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        private void OnEnable()
        {
            GameOff2024ScoreKeeper.OnGameEndedEvent += OnGameEndedEvent;
        }
        
        private void OnDisable()
        {
            GameOff2024ScoreKeeper.OnGameEndedEvent -= OnGameEndedEvent;
        }

        protected override void Awake()
        {
            base.Awake();
            
            _canvasGroup.alpha = 0;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
        }
        
        public void Show()
        {
            _canvasGroup.alpha = 1;
        }

        private void OnGameEndedEvent(GameOff2024Score score)
        {
            // Display score as a percentage
            _completionPercentageText.text = $"{GetCompletionPercentage(score):0}%";
            
            // display gameplay time as minutes and seconds
            _gameplayTimeText.text = $"{score.CompletionTime / 60:00}:{score.CompletionTime % 60:00}";
            
            _canvasGroup.alpha = 1;
        }
        
        private float GetCompletionPercentage(GameOff2024Score score)
        {
            // Consider just completing the game as 10% and the rest as optional collectables
            return 10f + 90f * ((float)score.OptionalCollectablesCollected / GameOff2024Statics.GetOptionalCollectableTotal());
        }
    }
}