using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = nameof(AudioSettingsPage), menuName = "Lavgine/UI/AudioSettings")]
public class AudioSettingsPage : SettingsPage
{
    protected override IEnumerator OnOpen(VisualElement content)
    {
        //content.RegisterSlider(nameof(GameSettings.Master),        evt => GameSettings.Master         = evt.newValue, GameSettings.Master);
        //content.RegisterSlider(nameof(GameSettings.SoundEffects),  evt => GameSettings.SoundEffects   = evt.newValue, GameSettings.SoundEffects);
        //content.RegisterSlider(nameof(GameSettings.UserInterface), evt => GameSettings.UserInterface  = evt.newValue, GameSettings.UserInterface);
        //content.RegisterSlider(nameof(GameSettings.Music),         evt => GameSettings.Music          = evt.newValue, GameSettings.Music);
        //content.RegisterSlider(nameof(GameSettings.Voice),         evt => GameSettings.Voice          = evt.newValue, GameSettings.Voice);
        //content.RegisterSlider(nameof(GameSettings.Ambient),       evt => GameSettings.Ambient        = evt.newValue, GameSettings.Ambient);

        yield return null;
    }
}