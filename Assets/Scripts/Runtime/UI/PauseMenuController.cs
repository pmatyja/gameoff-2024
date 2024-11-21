using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuController : Singleton<PauseMenuController>
{
    [Readonly]
    private UIDocument ui;

    public const float DefaultVolume = 0.75f;

    [SerializeField] private SliderValue[] defaultVolumes = new[]
    {
        new SliderValue(nameof(Master), DefaultVolume),
        new SliderValue(nameof(Sfx), DefaultVolume),
        new SliderValue(nameof(Music), DefaultVolume),
        new SliderValue(nameof(Ambient), DefaultVolume),
        new SliderValue(nameof(Voice), DefaultVolume),
    };

    [SerializeField]
    [Readonly]
    [Range(0, 1)]
    private float master = DefaultVolume;
    private Slider masterSlider;
    public float Master
    { 
        get => this.GetValue(this.masterSlider, ref this.master);
        set => this.SetValue(this.masterSlider, ref this.master, value);
    }

    [SerializeField]
    [Readonly]
    [Range(0, 1)]
    private float sfx = DefaultVolume;
    private Slider sfxSlider;
    public float Sfx
    { 
        get => this.GetValue(this.sfxSlider, ref this.sfx);
        set => this.SetValue(this.sfxSlider, ref this.sfx, value);
    }

    [SerializeField]
    [Readonly]
    [Range(0, 1)]
    private float music = DefaultVolume;
    private Slider musicSlider;
    public float Music
    { 
        get => this.GetValue(this.musicSlider, ref this.music);
        set => this.SetValue(this.musicSlider, ref this.music, value);
    }

    [SerializeField]
    [Readonly]
    [Range(0, 1)]
    private float ambient = DefaultVolume;
    private Slider ambientSlider;
    public float Ambient
    { 
        get => this.GetValue(this.ambientSlider, ref this.ambient);
        set => this.SetValue(this.ambientSlider, ref this.ambient, value);
    }

    [SerializeField]
    [Readonly]
    [Range(0, 1)]
    private float voice = DefaultVolume;
    private Slider voiceSlider;
    public float Voice
    { 
        get => this.GetValue(this.voiceSlider, ref this.voice);
        set => this.SetValue(this.voiceSlider, ref this.voice, value);
    }

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float fadeDuration = 0.25f;

    [SerializeField]
    [InputActionMap]
    private string pauseMenu;

    private VisualElement root;
    private int target = 0;

    public void Toggle()
    {
        if (this.target == 0)
            this.Open();
        else
            this.Close();
    }

    public void Open()
    {
        if (this.target == 1)
        {
            return;
        }
        
        this.target = 1;

        EventBus.Raise(this, new UIEventParameters
        {
            Action = UIAction.OpenMenu
        });
    }

    public void Close()
    {
        if (this.target == 0)
        {
            return;
        }

        this.target = 0;
        
        EventBus.Raise(this, new UIEventParameters
        {
            Action = UIAction.CloseMenu
        });
    }

    private void Start()
    {
        this.ui = this.GetComponent<UIDocument>();
        this.root = this.ui.rootVisualElement.Get<VisualElement>("Root") ?? this.ui.rootVisualElement;

        this.root.RegisterSlider(out this.masterSlider,     nameof(this.Master),     evt => this.SetValue(evt, ref this.master),  this.GetValue(nameof(this.Master),    ref this.master));
        this.root.RegisterSlider(out this.sfxSlider,        nameof(this.Sfx),        evt => this.SetValue(evt, ref this.sfx),     this.GetValue(nameof(this.Sfx),       ref this.sfx));
        this.root.RegisterSlider(out this.musicSlider,      nameof(this.Music),      evt => this.SetValue(evt, ref this.music),   this.GetValue(nameof(this.Music),     ref this.music));
        this.root.RegisterSlider(out this.voiceSlider,      nameof(this.Voice),      evt => this.SetValue(evt, ref this.voice),   this.GetValue(nameof(this.Voice),     ref this.voice));
        this.root.RegisterSlider(out this.ambientSlider,    nameof(this.Ambient),    evt => this.SetValue(evt, ref this.ambient), this.GetValue(nameof(this.Ambient),   ref this.ambient));

        if (this.root.TryGet<VisualElement>("Close", out var close, true))
        {
            close.OnClick(evt =>this.Close());
        }

        if (this.root.TryGet<VisualElement>("MainMenu", out var mainMenu, true))
        {
            mainMenu.OnClick(evt =>
            {
                this.Close();
                // add going back to main menu
            });
        }
    }

    public void Update()
    {
        this.root.Fade(this.target, this.fadeDuration);

        if (InputManager.Released(this.pauseMenu))
        {
            this.Toggle();
        }
    }

    private float GetValue(string name, ref float previewField)
    {
        GameSettings.TryGet(GameSettingsSubsystem.Audio, name, out var value, 0.75f);
        return previewField = value;
    }

    private float GetValue(Slider slider, ref float previewField)
    {
        return this.GetValue(slider.label, ref previewField);
    }

    private void SetValue(Slider slider, ref float previewField, float value)
    {
        slider.value = Mathf.InverseLerp(slider.lowValue, slider.highValue, value);

        previewField = value;
        
        GameSettings.Set(GameSettingsSubsystem.Audio, slider.label, value);
    }

    private void SetValue(ChangeEvent<float> evt, ref float previewField)
    {
        this.SetValue(evt.target as Slider, ref previewField, evt.newValue);
    }

    private void OnValidate()
    {
        // Do not apply changes to the default volumes when in play mode
        if (Application.isPlaying)
        {
            return;
        }
        
        foreach (var sliderValue in this.defaultVolumes)
        {
            switch (sliderValue.Name)
            {
                case "Master": this.master = sliderValue.Value; break;
                case "Sfx": this.sfx = sliderValue.Value; break;
                case "Music": this.music = sliderValue.Value; break;
                case "Ambient": this.ambient = sliderValue.Value; break;
                case "Voice": this.voice = sliderValue.Value; break;
                default: return;
            }

            GameSettings.Set(GameSettingsSubsystem.Audio, sliderValue.Name, sliderValue.Value);
        }
    }

    [Serializable]
    public class SliderValue
    {
        [Readonly]
        public string Name;

        [Range(0.0f, 1.0f)]
        public float Value;

        public SliderValue(string name, float value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    public enum UIAction
    {
        OpenMenu,
        CloseMenu
    }

    public struct UIEventParameters
    {
        public UIAction Action;
    }
}
