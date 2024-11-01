using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[Serializable]
public class MainMenu : UiElement
{
    [Header("Input")]

    [SerializeField]
    [InputActionMap]
    private string inputUp;

    [SerializeField]
    [InputActionMap]
    private string inputDown;

    [SerializeField]
    [InputActionMap]
    private string inputConfirm;

    [SerializeField]
    [InputActionMap]
    private string inputBack;

    [SerializeField]
    private bool loadDefaultScene = true;

    [SerializeField]
    private MusicSO music;
    
    [SerializeField]
    private AudioResourceSO onStart;

    [SerializeField]
    private AudioResourceSO onExit;
    
    [SerializeField]
    private AudioResourceSO onBack;

    [SerializeField]
    private AudioResourceSO onConfirmSelection;

    [SerializeField]
    private AudioResourceSO onSelectionChanged;

    private int selectedIndex;
    private bool mouseClick = false;

    private const string ButtonHoverClass = "button-selected";

    public override bool IsVisible(GameMode mode)
    {
        return mode == GameMode.Menu;
    }

    public override void Enable(MonoBehaviour behaviour, VisualElement root)
    {
        base.Enable(behaviour, root.Q<VisualElement>(nameof(MainMenu)));

        this.RegisterPage("Credits");
        this.RegisterPage("Settings");
    }

    public override void Disable()
    {
        base.Disable();
    }

    public override IEnumerator Animate()
    {
        this.music?.Play();

        this.Root.SetOpacity(0.0f);

        var main = this.Root.Q<VisualElement>("Main");

        main.SetOpacity(0.0f);

        var mouseClick = false;

        if (main.TryGet<VisualElement>("Menu", out var page, true))
        {
            page.HoverChild(this.selectedIndex, ButtonHoverClass);

            foreach (var child in page.Children())
            {
                child.OnClick(evt =>
                {
                    mouseClick = true;
                    this.onConfirmSelection?.Play();

                    EventBus.Raise<MouseClickEvent>(this, new MouseClickEvent
                    {
                        Element = child
                    });
                });

                child.OnHover(evt =>
                {
                    this.selectedIndex = child.parent.IndexOf(child);
                    page.HoverChild(this.selectedIndex, ButtonHoverClass);
                    this.onSelectionChanged?.Play();

                    EventBus.Raise<MouseHoverEvent>(this, new MouseHoverEvent
                    {
                        Element = child
                    });
                });
            }

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                if (page.TryGet<VisualElement>("Menu-Exit", out var menuExit, true))
                {
                    menuExit.RemoveFromHierarchy();
                }
            }
        }

        EventBus.Raise<FadeInEventParameters>(this, new FadeInEventParameters() { Id = "menu" });

        yield return this.Root.FadeIn(1.0f);
        yield return main.FadeIn();

        while (this.Behaviour.isActiveAndEnabled)
        {
            if (InputManager.Released(this.inputUp))
            {
                if (this.selectedIndex > 0)
                {
                    this.selectedIndex--;
                    page.HoverChild(this.selectedIndex, ButtonHoverClass);
                    this.onSelectionChanged?.Play();

                    EventBus.Raise<MouseHoverEvent>(this, new MouseHoverEvent
                    {
                        Element = page.Children().Skip(this.selectedIndex).FirstOrDefault()
                    });
                }
            }

            if (InputManager.Released(this.inputDown))
            {
                if (this.selectedIndex < page.childCount - 1)
                {
                    this.selectedIndex++;
                    page.HoverChild(this.selectedIndex, ButtonHoverClass);
                    this.onSelectionChanged?.Play();

                    EventBus.Raise<MouseHoverEvent>(this, new MouseHoverEvent
                    {
                        Element = page.Children().Skip(this.selectedIndex).FirstOrDefault()
                    });
                }
            }

            if (mouseClick || InputManager.Released(this.inputConfirm))
            {
                mouseClick = false;

                this.onConfirmSelection?.Play();

                var selected = page.Children().Skip(this.selectedIndex).FirstOrDefault();

                EventBus.Raise<MouseClickEvent>(this, new MouseClickEvent
                {
                    Element = selected
                });

                this.mouseClick = false;

                if (selected.name == "Menu-Start")
                {
                    this.onStart?.Play();

                    EventBus.Raise<FadeOutEventParameters>(this, new FadeOutEventParameters() { Id = "game" });

                    yield return this.Root.FadeOut();

                    EventBus.Raise<GameSceneEventParameters>(this, new GameSceneEventParameters());

                    if (this.loadDefaultScene)
                    {
                        SceneManager.LoadScene("Game");
                    }

                    yield break;
                }
                else if (selected.name == "Menu-Exit")
                {
                    EventBus.Raise<FadeOutEventParameters>(this, new FadeOutEventParameters() { Id = "menu" });
                    yield return this.Root.FadeOut();

                    Application.Quit();
                    yield break;
                }
                else
                {
                    var pageName = selected.name.Replace("Menu-", string.Empty);

                    if (this.Root.TryGet<VisualElement>(pageName, out var subpage, true))
                    {
                        EventBus.Raise<FadeOutEventParameters>(this, new FadeOutEventParameters() { Id = "menu" });
                        yield return main.FadeOut();

                        EventBus.Raise<FadeInEventParameters>(this, new FadeInEventParameters() { Id = pageName });
                        yield return subpage.FadeIn();

                        while (this.Behaviour.isActiveAndEnabled)
                        {
                            if (this.mouseClick || InputManager.Released(this.inputConfirm) || InputManager.Released(this.inputBack))
                            {
                                this.mouseClick = false;
                                this.onBack?.Play();
                                break;
                            }

                            yield return base.Animate();
                        }

                        EventBus.Raise<FadeOutEventParameters>(this, new FadeOutEventParameters() { Id = pageName });
                        yield return subpage.FadeOut();

                        EventBus.Raise<FadeInEventParameters>(this, new FadeInEventParameters() { Id = "menu" });
                        yield return main.FadeIn();
                    }
                }
            }

            yield return base.Animate();
        }
    }

    private void RegisterPage(string name)
    {
        if (this.Root.TryGet<VisualElement>(name, out var page, true))
        {
            if (page.TryGet<VisualElement>("Menu-Back", out var back, true))
            {
                back.OnClick(evt =>
                {
                    this.mouseClick = true;

                    EventBus.Raise<MouseClickEvent>(this, new MouseClickEvent
                    {
                        Element = back
                    });
                });

                back.OnHover(evt =>
                {
                    EventBus.Raise<MouseHoverEvent>(this, new MouseHoverEvent
                    {
                        Element = back
                    });
                });
            }
        }
    }
}
