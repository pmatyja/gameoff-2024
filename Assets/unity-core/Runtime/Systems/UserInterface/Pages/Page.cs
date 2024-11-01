using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Page : ScriptableObject
{
    [SerializeField]
    [Readonly]
    [NotNull]
    protected VisualTreeAsset template;
    public VisualTreeAsset Template => this.template;

    [SerializeField]
    private bool defaultCloseBehaviour;
    public bool DefaultCloseBehaviour => this.defaultCloseBehaviour;

    [SerializeField]
    private GameMode gameMode;
    public GameMode GameMode => this.gameMode;

    [SerializeField]
    [Range(0f, 1f)]
    private float transitionDuration = 0.25f;
    public float TransitionDuration { get => this.transitionDuration; set => this.transitionDuration = Mathf.Clamp01(value); }

    public virtual VisualElement Create()
    {
        return template.CloneTree().Children().FirstOrDefault();
    }

    public virtual IEnumerator Open(VisualElement content)
    {
        content.Clear();

        this.template.CloneTree(content);

        content.SetOpacity(0.0f);

        yield return this.OnOpen(content);
        yield return content.FadeIn();
    }

    public virtual void OnUpdate(VisualElement element)
    {
        element.Fade(GameManager.Mode == this.gameMode ? 1.0f : 0.0f, this.transitionDuration);
    }

    public virtual IEnumerator Close(VisualElement content)
    {
        yield return this.OnClose(content);
        yield return content.FadeOut();

        content.RemoveFromHierarchy();
    }

    protected void Register<T>(VisualElement content, string id, EventCallback<T> callback, TrickleDown trickleDown = TrickleDown.TrickleDown) where T : EventBase<T>, new()
    {
        if (content.TryGet<VisualElement>(id, out var element))
        {
            element.RegisterCallback<T>(callback, trickleDown);
        }
        else
        {
            Debug.LogWarning($"RegisterSlider: Element '{id}' not found");
        }
    }

    protected void RegisterSlider(VisualElement content, string id, EventCallback<ChangeEvent<float>> callback, float defaultValue = 1.0f)
    {
        if (content.TryGet<Slider>(id, out var element))
        {
            element.RegisterValueChangedCallback<float>(callback);
            element.value = defaultValue;
        }
        else
        {
            Debug.LogWarning($"RegisterSlider: Element '{id}' not found");
        }
    }

    protected abstract IEnumerator OnOpen(VisualElement content);
    protected abstract IEnumerator OnClose(VisualElement content);

    [ContextMenu("Auto Initialize")]
    protected abstract void OnValidate();
}
