using System;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    public TranslationsSO Translations;

    [SerializeField]
    private GameMode mode = GameMode.Gameplay;

    public static GameMode Mode
    {
        get => GameManager.Instance.mode;
        set
        {
            GameManager.Instance.mode = value;

            if (value == GameMode.Cutscene)
            {
                AudioSources.Stop(AudioLayers.VoiceLine);
            }
            
            EventBus.Raise(null, new GameModeEventParameters { Mode = GameManager.Instance.mode });
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // Announce what the current game mode is at start
        Mode = mode;
    }

    private void OnValidate()
    {
        if (this.Translations == null)
        {
            Translations = EditorOnly.LoadAsset<TranslationsSO>("Assets/Resources/Shared/TRA_English.asset");
        }

        this.UpdateLangauge();
    }

    [ContextMenu("Refresh Translations")]
    private void UpdateLangauge()
    {
        if (this.Translations != null)
        {
            if (Application.isPlaying)
            {
                EventBus.Raise(null, new LanguageChangedEventParameters
                {
                    Translations = this.Translations
                });
            }
        }
    }
}