using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = nameof(EndScreenPage), menuName = "Lavgine/UI/End Screen")]
public class EndScreenPage : SettingsPage
{
    public Action OnStart;
    public Action OnCredits;
    public Action OnExit;

    protected override IEnumerator OnOpen(VisualElement content)
    {
        this.Register<MouseDownEvent>(content, "Start", _ =>
        {
            this.OnStart?.Invoke();
        });
        this.Register<MouseDownEvent>(content, "Credits", _ =>
        {
            this.OnCredits?.Invoke();
        });

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            content.Q<VisualElement>("Ext")?.RemoveFromHierarchy();
        }
        else
        {
            this.Register<MouseDownEvent>(content, "Exit", _ =>
            {
                this.OnExit?.Invoke();
            });
        }

        yield return null;
    }

    protected override IEnumerator OnClose(VisualElement content)
    {
        yield return null;
    }

    protected override void OnValidate()
    {
        EditorOnly.LoadAsset($"Assets/Resources/UI/EndScreen/{this.name}.uxml", out this.template);
    }
}
