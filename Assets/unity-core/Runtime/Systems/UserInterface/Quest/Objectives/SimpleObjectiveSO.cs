using UnityEngine;

[CreateAssetMenu(fileName = nameof(SimpleObjectiveSO), menuName = "Lavgine/Database.Questing/Quest Objective (Simple)")]
public class SimpleObjectiveSO : ObjectiveSO
{
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

        EventBus.Raise(this, new ObjectiveSucceededEventParameters
        {
            Objective = this
        });

        this.Status = ProgressStatus.Successed;
    }
}
