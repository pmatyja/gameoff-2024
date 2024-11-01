using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Nodes.Actions.Conversation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
[DisallowMultipleComponent]
public class UserInterface : Singleton<UserInterface>
{
    [SerializeField]
    [Readonly]
    private UIDocument document;

    [SerializeField]
    [NotNull("File 'Shared/PAS_PanelSettings.asset' not found")]
    [Readonly]
    private PanelSettings panelSettings;

    [Header("Pause Menu")]

    [SerializeField]
    [Readonly]
    private bool hasSettingsMenu = false;

    [SerializeField]
    [Readonly]
    private bool showSettingsMenu = false;

    [SerializeField]
    [InputActionMap]
    private bool loadMainMenuScene;

    [SerializeField]
    [InputActionMap]
    private string settingsMenuButtonGameplay;

    [SerializeField]
    [InputActionMap]
    private string settingsMenuButtonCutscene;

    [SerializeField]
    [InputActionMap]
    private string settingsMenuButtoneSettingsMenu;

    [Header("Fade")]

    [SerializeField]
    [Readonly]
    private VisualElement fade;

    [SerializeField]
    [Readonly]
    [Range(0.0f, 1.0f)]
    private float fadeTarget = 0.0f;

    [SerializeField]
    [Range(0.01f, 10.0f)]
    private float fadeDuration = 1.0f;

    [SerializeReference]
    [TypeInstanceSelector]
    private List<UiElement> elements = new();

    private VisualElement root;
    private VisualElement gameplay;
    private VisualElement cutscene;
    private VisualElement menu;
    private VisualElement userInterface;
    private VisualElement settings;

    private List<VisualElement> roots = new();

    private object pageLocker = new();
    private ConcurrentStack<PageEntry> stack = new();
    private ConcurrentQueue<PageEventsParameters> actions = new();

    public IEnumerator PlayConversation(ConversationNode conversation, IEnumerable<Choice> choices, CoroutineResult<ChoiceRef> result)
    {
        var dialog = this.elements.FirstOrDefault(x => x is DialogSystem) as DialogSystem;
        if (dialog == null)
        {
            yield break;
        }

        yield return dialog.PlayConversation(conversation, choices, result);
    }

    private class PageEntry
    {
        public Page Page;
        public VisualElement Element;
    }

    private void OnEnable()
    {
        this.actions.Clear();

        foreach (var entry in this.stack)
        {
            entry.Element.RemoveFromHierarchy();
        }

        this.stack.Clear();
        
        this.root           = this.document.rootVisualElement.Q<VisualElement>("Root");

        this.gameplay       = this.document.rootVisualElement.Q<VisualElement>(GameMode.Gameplay.ToString());
        this.cutscene       = this.document.rootVisualElement.Q<VisualElement>(GameMode.Cutscene.ToString());
        this.menu           = this.document.rootVisualElement.Q<VisualElement>(GameMode.Menu.ToString());

        this.roots.Add(this.gameplay);
        this.roots.Add(this.cutscene);
        this.roots.Add(this.menu);

        this.root.TryGet("UI", out this.userInterface);

        // Pause Menu pop up

        if (this.root.TryGet("Settings", out this.settings, false))
        {
            this.hasSettingsMenu = true;
            this.settings.SetOpacity(0.0f);

            Logger.Info("UI: Settings present");

            if (this.settings.TryGet<VisualElement>("MainMenu", out var mainMenuButton))
            {
                mainMenuButton.OnClick(evt =>
                {
                    if (loadMainMenuScene)
                    {
                        SceneManager.LoadScene("MainMenu");
                    }

                    EventBus.Raise<MouseClickEvent>(this, new MouseClickEvent
                    {
                        Element = mainMenuButton
                    });

                    EventBus.Raise<MainMenuSceneEventParameters>(this);
                });

                EventBus.Raise<MouseHoverEvent>(this, new MouseHoverEvent
                {
                    Element = mainMenuButton
                });
            }

            if (this.settings.TryGet<VisualElement>("Close", out var closeButton))
            {
                closeButton.OnClick(evt =>
                {
                    this.showSettingsMenu = false;

                    EventBus.Raise(this, new EnableInGameUIEventParameters
                    {
                        State = true
                    });

                    EventBus.Raise<MouseClickEvent>(this, new MouseClickEvent
                    {
                        Element = closeButton
                    });
                });

                EventBus.Raise<MouseHoverEvent>(this, new MouseHoverEvent
                {
                    Element = closeButton
                });
            }

            if (this.settings.TryGet<VisualElement>("Content", out var content))
            {
                Logger.Info("UI: Settings->Content present");

                foreach (var group in content.Children())
                {
                    if (Enum.TryParse<GameSettingsSubsystem>(group.name, true, out var subsystem) == false)
                    {
                        Logger.Warning($"UI: Settings->{group.name} is not a valid 'GameSettingsSubsystem' subsystem enum value");
                        continue;
                    }

                    var sliders = group.GetChildren<Slider>();

                    foreach (var slider in sliders)
                    {
                        var normalized = slider.name.Replace(" ", string.Empty);

                        Logger.Info($"UI: Settings->{group.name}->{normalized} slider present");

                        slider.RegisterSlider(value =>
                        {
                            GameSettings.Set<float>(subsystem, normalized, value.newValue);

                            EventBus.Raise<SliderValueChangedEvent>(this, new SliderValueChangedEvent
                            {
                                Element = slider,
                                Id = slider.name,
                                OldValue = value.previousValue,
                                NewValue = value.newValue
                            });
                        }, 
                        GameSettings.Get(subsystem, normalized, 1.0f));

                        EventBus.Raise<SliderValueChangedEvent>(this, new SliderValueChangedEvent
                        {
                            Element = slider,
                            Id = slider.name,
                            OldValue = slider.value,
                            NewValue = slider.value
                        });
                    }
                }
            }
        }

        this.root.TryGet("Fade", out this.fade, true);

        foreach (var element in this.elements)
        {
            element?.Enable(this, this.root);
        }

        EventBus.AddListener<PushPageEventsParameters>(this.OnPush);
        EventBus.AddListener<PopPageEventsParameters>(this.OnPop);
        EventBus.AddListener<LanguageChangedEventParameters>(this.OnLanguageChanged);
        EventBus.AddListener<FadeInEventParameters>(this.OnFadeIn);
        EventBus.AddListener<FadeOutEventParameters>(this.OnFadeOut);

        this.StartCoroutine(this.OnActions());
        this.StartCoroutine(this.OnUpdate());
    }

    private void OnDisable()
    {
        EventBus.RemoveListener<PushPageEventsParameters>(this.OnPush);
        EventBus.RemoveListener<PopPageEventsParameters>(this.OnPop);
        EventBus.RemoveListener<LanguageChangedEventParameters>(this.OnLanguageChanged);
        EventBus.RemoveListener<FadeInEventParameters>(this.OnFadeIn);
        EventBus.RemoveListener<FadeOutEventParameters>(this.OnFadeOut);

        this.StopAllCoroutines();

        this.actions.Clear();

        foreach (var entry in this.stack)
        {
            entry.Element.RemoveFromHierarchy();
        }

        this.stack.Clear();

        foreach (var element in this.elements)
        {
            element?.Disable();
        }
    }

    private void Update()
    {
        if (InputManager.Released(this.settingsMenuButtonGameplay) || InputManager.Released(this.settingsMenuButtonCutscene) || InputManager.Released(this.settingsMenuButtoneSettingsMenu))
        {
            this.showSettingsMenu = !this.showSettingsMenu;

            EventBus.Raise(this, new EnableInGameUIEventParameters
            {
                State = !this.showSettingsMenu
            });

            if (this.showSettingsMenu)
            {
                if (this.settings != null)
                {
                    if (this.settings.TryGet<VisualElement>("Content", out var content))
                    {
                        Logger.Info("UI: Settings->Content present");

                        foreach (var group in content.Children())
                        {
                            if (Enum.TryParse<GameSettingsSubsystem>(group.name, true, out var subsystem) == false)
                            {
                                continue;
                            }

                            var sliders = group.GetChildren<Slider>();

                            foreach (var slider in sliders)
                            {
                                var normalized = slider.name.Replace(" ", string.Empty);

                                slider.value = GameSettings.Get(subsystem, normalized, 1.0f);

                                Logger.Info($"UI: Settings->Content->{slider.name}={slider.value} loaded");
                            }
                        }
                    }
                }
            }
        }

        if (this.hasSettingsMenu)
        {
            if (GameManager.Mode != GameMode.Menu)
            {
                if (this.showSettingsMenu)
                {
                    this.settings.SetOpacity(1.0f);
                }
                else
                {
                    this.settings.SetOpacity(0.0f);
                }
            }
        }

        foreach (var root in this.roots)
        {
            if (root == null)
            {
                continue;
            }

            if (string.Equals(root.name, GameManager.Mode.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                root.Fade(1.0f);
            }
            else
            {
                root.Fade(0.0f);
            }
        }

        foreach (var element in this.elements)
        {
            element?.Update();
        }

        this.fade?.Fade(this.fadeTarget, this.fadeDuration);
    }

    private void OnValidate()
    {
        EditorOnly.LoadAsset("Assets/Resources/Shared/PAS_PanelSettings.asset", out this.panelSettings);

        this.document = this.GetComponent<UIDocument>();
        this.document.panelSettings = this.panelSettings;

        foreach (var element in this.elements)
        {
            element?.OnValidate();
        }
    }

    private void OnPush(object sender, PushPageEventsParameters parameters)
    {
        this.actions.Enqueue(parameters);
    }

    private void OnPop(object sender, PopPageEventsParameters parameters)
    {
        this.actions.Enqueue(parameters);
    }

    private void OnLanguageChanged(object sender, LanguageChangedEventParameters parameters)
    {
        this.root?.Localize(parameters.Translations);
    }

    private void OnFadeIn(object sender, FadeInEventParameters parameters)
    {
        this.fadeTarget = 0.0f;
    }

    private void OnFadeOut(object sender, FadeOutEventParameters parameters)
    {
        this.fadeTarget = 1.0f;
    }

    private IEnumerator OnUpdate()
    {
        while (this.isActiveAndEnabled)
        {
            foreach (var entry in this.stack)
            {
                entry.Page.OnUpdate(entry.Element);
            }

            yield return null;
        }
    }

    private IEnumerator OnActions()
    {
        while (this.isActiveAndEnabled)
        {
            if (this.actions.TryDequeue(out var action))
            {
                if (action is PushPageEventsParameters push)
                {
                    if (push.Page == null)
                    {
                        Debug.LogWarning("Push UI page is null");
                    }
                    else
                    {
                        var element = new VisualElement();

                        this.stack.Push(new PageEntry { Page = push.Page, Element = element });
                        this.userInterface.Add(element);

                        yield return push.Page.Open(element);
                    }
                }
                else if (action is PopPageEventsParameters)
                {
                    if (this.stack.TryPeek(out var peek))
                    {
                        PageEntry entry = default;

                        lock (this.pageLocker)
                        {
                            if (peek.Page.DefaultCloseBehaviour)
                            {
                                this.stack.TryPop(out entry);
                            }
                        }

                        if (entry != null)
                        {
                            yield return entry.Page.Close(entry.Element);
                            entry.Element.RemoveFromHierarchy();
                        }
                    }
                }
            }

            yield return null;
        }
    }
}
