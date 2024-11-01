using System;
using System.ComponentModel;

[Serializable]
[DisplayName("Choice")]
public class ChoiceBooleanValueSource : IBooleanValueSource
{
    [ChoiceTrackerSelector(Label = LabelState.Hidden)]
    public string Marker;

    [DisableIf(nameof(Marker))]
    public bool ExpectedValue = true;

    public bool GetValue()
    {
        foreach (var choice in ChoiceTrackerSO.Instance.Choices)
        {
            if (choice.Name == Marker)
            {
                return choice.State == this.ExpectedValue;
            }
        }

        Logger.Warning($"Choice Tracker does not contain '{Marker}' marker");
        return this.ExpectedValue;
    }
}