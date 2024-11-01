using UnityEngine;

[CreateAssetMenu(fileName = nameof(CountObjectiveSO), menuName = "Lavgine/Database.Questing/Quest Objective (Count)")]
public class CountObjectiveSO : ObjectiveSO
{
    [Range(0, 100)]
    public int Count = 0;

    [Range(1, 100)]
    public int Required = 1;

    protected override void OnObjectiveAdvanceEvent(object sender, ObjectiveAdvanceEventParameters parameters)
    {
        if (parameters.Objective != this)
        {
            return;
        }

        if (this.Status != ProgressStatus.InProgress)
        {
            return;
        }

        this.Count = Mathf.Min(this.Count + 1, this.Required);

        if (this.Count == this.Required)
        {
            EventBus.Raise(this, new ObjectiveSucceededEventParameters
            {
                Objective = this
            });

            this.Status = ProgressStatus.Successed;
        }
    }

    protected override string GetName()
    {
        return $"{this.Count} / {this.Required} {this.Name}";
    }
}
