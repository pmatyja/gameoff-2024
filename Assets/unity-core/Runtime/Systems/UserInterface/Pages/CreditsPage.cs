using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = nameof(CreditsPage), menuName = "Lavgine/UI/Credits")]
public class CreditsPage : SettingsPage
{
    public Action OnBack;

    protected override IEnumerator OnOpen(VisualElement content)
    {
        this.Register<MouseDownEvent>(content, "Back", _ =>
        {
            this.OnBack?.Invoke();
        });

        yield return null;
    }

    protected override IEnumerator OnClose(VisualElement content)
    {
        yield return null;
    }

    protected override void OnValidate()
    {
        EditorOnly.LoadAsset($"Assets/Resources/UI/MainMenu/{this.name}.uxml", out this.template);
    }
}