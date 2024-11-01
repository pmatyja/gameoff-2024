using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = nameof(AudioSettingsPage), menuName = "Lavgine/UI/Page: Settings")]
public abstract class SettingsPage : Page
{
    protected override IEnumerator OnOpen(VisualElement content)
    {
        //content.RegisterSlider(nameof(GameSettings.Master), evt => GameSettings.Master = evt.newValue, GameSettings.Master);
        //content.RegisterSlider(nameof(GameSettings.SoundEffects), evt => GameSettings.SoundEffects = evt.newValue, GameSettings.SoundEffects);
        //content.RegisterSlider(nameof(GameSettings.UserInterface), evt => GameSettings.UserInterface = evt.newValue, GameSettings.UserInterface);
        //content.RegisterSlider(nameof(GameSettings.Music), evt => GameSettings.Music = evt.newValue, GameSettings.Music);
        //content.RegisterSlider(nameof(GameSettings.Voice), evt => GameSettings.Voice = evt.newValue, GameSettings.Voice);
        //content.RegisterSlider(nameof(GameSettings.Ambient), evt => GameSettings.Ambient = evt.newValue, GameSettings.Ambient);

        content.OnClick(evt =>
        {
            SceneManager.LoadScene("MainMenu");
        });

        content.OnClick(evt =>
        {
            EventBus.Raise(null, new PopPageEventsParameters { Page = this });
        });

        yield return null;
    }

    protected override IEnumerator OnClose(VisualElement content)
    {
        yield return null;
    }

    protected override void OnValidate()
    {
        EditorOnly.LoadAsset($"Assets/Resources/Shared/Settings/{this.name}.uxml", out this.template);
    }
}