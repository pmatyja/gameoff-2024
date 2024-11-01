using Nodes.Value;
using System;
using UnityEngine;

[Serializable]
public class Choice
{
    [Parameter]
    public ValueNode<bool> Condition;

    public bool HideOnFailedCondition;

    [TextArea(3, 3)]
    [HideLabel]
    public string Text;

    public bool IsPrimary;

    [Range(0, 16)]
    public short OrderId;

    [ChoiceTrackerSelector(Label = LabelState.SeparateLine)]
    public string SetMarker;
}
