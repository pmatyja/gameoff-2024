using UnityEngine.UIElements;

public struct SliderValueChangedEvent
{
    public Slider Element;
    public string Id;
    public float OldValue;
    public float NewValue;
}