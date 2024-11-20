using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuController : MonoBehaviour
{
    [Readonly]
    private UIDocument root;

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

    private void Start()
    {
        this.root = this.GetComponent<UIDocument>();

        this.root.rootVisualElement.RegisterSlider(out this.masterSlider,     nameof(this.Master),     evt => this.SetValue(evt, ref this.master),  this.GetValue(nameof(this.Master),    ref this.master));
        this.root.rootVisualElement.RegisterSlider(out this.sfxSlider,        nameof(this.Sfx),        evt => this.SetValue(evt, ref this.sfx),     this.GetValue(nameof(this.Sfx),       ref this.sfx));
        this.root.rootVisualElement.RegisterSlider(out this.musicSlider,      nameof(this.Music),      evt => this.SetValue(evt, ref this.music),   this.GetValue(nameof(this.Music),     ref this.music));
        this.root.rootVisualElement.RegisterSlider(out this.voiceSlider,      nameof(this.Voice),      evt => this.SetValue(evt, ref this.voice),   this.GetValue(nameof(this.Voice),     ref this.voice));
        this.root.rootVisualElement.RegisterSlider(out this.ambientSlider,    nameof(this.Ambient),    evt => this.SetValue(evt, ref this.ambient), this.GetValue(nameof(this.Ambient),   ref this.ambient));
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
        public string Name;
        public float Value;

        public SliderValue(string name, float value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
