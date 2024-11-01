using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ChoiceTrackerSO), menuName = "Lavgine/Database.Questing/Choice Tracker")]
public class ChoiceTrackerSO : ScriptableObject
{
	public static ChoiceTrackerSO Instance
	{
		get
		{
			if (instance == null)
			{
                instance = EditorOnly.LoadOrCreateAsset<ChoiceTrackerSO>("Assets/Resources/Shared/CHT_ChoiceTracker.asset");
			}

			return instance;
		}
    }

    public List<ChoiceMarker> Choices = new();

    private static ChoiceTrackerSO instance;

    public void MarkChoice(string id)
	{
		if (string.IsNullOrWhiteSpace(id))
		{
			return;
		}

        if (this.Choices == null)
        {
            return;
        }
		
		var index = this.Choices.FindIndex(x => x.Name == id);
		if (index > -1)
		{
			this.Choices[index] = new ChoiceMarker
			{
                Name = id,
				State = true
			};
		}
    }

    [ContextMenu("Reset values")]
    public void Reset()
    {
        if (this.Choices == null)
        {
            return;
        }

        for (var index = 0; index < this.Choices.Count; index++)
        {
            this.Choices[index] = new ChoiceMarker
            {
                Name = this.Choices[index].Name,
                State = false
            };
        }
    }

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("More than one Choice Tracker file exits");
        }

        this.Reset();
    }

    private void OnValidate()
	{
        instance = EditorOnly.LoadOrCreateAsset<ChoiceTrackerSO>("Assets/Resources/Shared/CHT_ChoiceTracker.asset");
	}
}