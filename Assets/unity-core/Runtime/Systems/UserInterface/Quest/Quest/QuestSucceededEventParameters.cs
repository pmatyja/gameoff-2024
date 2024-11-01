using System;

[Serializable]
public struct QuestSucceededEventParameters
{
    public QuestSO Quest;
    public bool FailRemainingObjectives;
}
