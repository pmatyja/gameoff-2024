using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class AmbientDialogSystem : UiElement
{
    private object locker = new();
    private ConcurrentQueue<DialogEventParameters> dialogs = new();

    public override bool IsVisible(GameMode mode)
    {
        return mode == GameMode.Gameplay || mode == GameMode.Settings;
    }

    public override void Enable(MonoBehaviour behaviour, VisualElement root)
    {
        base.Enable(behaviour, root.Q<VisualElement>(nameof(AmbientDialogSystem)));

        EventBus.AddListener<DialogEventParameters>(this.OnDialog);
    }

    public override void Disable()
    {
        base.Disable();

        EventBus.RemoveListener<DialogEventParameters>(this.OnDialog);
    }

    public override IEnumerator Animate()
    {
        this.Root.SetOpacity(0.0f);

        var content = this.Root.Q<Label>("Content");

        while (this.Behaviour.isActiveAndEnabled)
        {
            if (GameManager.Mode == GameMode.Gameplay)
            {
                if (this.dialogs.TryDequeue(out var dialog))
                {
                    content.text = dialog.Content?.Localize();

                    yield return this.Root.FadeIn();

                    if (dialog.VoiceLine == null)
                    {
                        yield return Wait.Seconds(UITheme.GetTextDisplayTime(content.text));
                    }
                    else
                    {
                        yield return Wait.Seconds(dialog.VoiceLine.Duration);
                    }

                    yield return this.Root.FadeOut();
                }
            }

            yield return base.Animate();
        }
    }

    private void OnDialog(object sender, DialogEventParameters parameters)
    {
        if (GameManager.Mode != GameMode.Gameplay)
        {
            return;
        }

        lock (this.locker)
        {
            if (this.dialogs.TryPeek(out var dialog))
            {
                if (dialog.Priority > parameters.Priority)
                {
                    return;
                }

                this.dialogs.Clear();
            }

            this.dialogs.Enqueue(parameters);
        }
    }
}