public class ChoiceRef
{
    public readonly int Index;
    public readonly string Text;
    public readonly bool IsPrimary;
    public readonly bool IsVisible;
    public readonly bool IsBlocked;
    public readonly string Marker;

    public ChoiceRef(int index, Choice choice)
    {
        this.Index = index;
        this.Text = choice.Text;
        this.IsPrimary = choice.IsPrimary;
        this.IsVisible = true;

        if (choice.Condition != null)
        {
            if (choice.Condition.GetValue() == false)
            {
                this.IsBlocked = true;
                this.IsVisible = choice.HideOnFailedCondition == false;
            }
        }

        this.Marker = choice.SetMarker;
    }

    public bool CanPickChoice()
    {
        return this.IsVisible && this.IsBlocked == false;
    }

    public void Pick()
    {
        ChoiceTrackerSO.Instance?.MarkChoice(this.Marker);
    }
}