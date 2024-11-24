using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HudController : MonoBehaviour
{
    [SerializeField]
    [Readonly]
    private UIDocument document;

    [SerializeField]
    private PauseMenuController pauseMenu;

    private void Start()
    {
        this.document = this.GetComponent<UIDocument>();

        if (this.document.rootVisualElement.TryGet<VisualElement>("Pause", out var pause))
        {
            pause.OnClick(evt =>
            {
                EventBus.Raise<PauseMenuController.OpenPauseMenuEventsParameters>(this);
            });
        }
    }

    private void OnEnable()
    {
        EventBus.AddListener<ItemCollectedEventsParameters>(this.OnItemCollected);
    }

    private void OnDisable()
    {
        EventBus.RemoveListener<ItemCollectedEventsParameters>(this.OnItemCollected);
    }

    private void OnItemCollected(object sender, ItemCollectedEventsParameters parameters)
    {
        switch (parameters.Type)
        {
            case ItemType.Collectable:
                //this.collectable.Text = this.collectables.T
                break;
        }
    }
}

public enum ItemType
{
    Collectable,
    Key
}

public struct ItemCollectedEventsParameters
{
    public ItemType Type;
    public int Count;
}
