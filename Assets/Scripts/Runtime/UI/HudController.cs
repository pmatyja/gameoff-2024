using System;
using Common;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HudController : MonoBehaviour
{
    [SerializeField]
    [Readonly]
    private UIDocument document;

    [SerializeField]
    [Readonly]
    private int coinsCounter;

    [SerializeField]
    [Range(0, 100)]
    private int maxCoinsCounter;

    [SerializeField]
    private CubeDef[] sprites;

    private Label coinsCounterElement;

    private void Start()
    {
        this.document = this.GetComponent<UIDocument>();

        this.document.rootVisualElement.TryGet("Coins", out this.coinsCounterElement, true);

        this.UpdateCoinsCounter();

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
        EventBus.AddListener<CubeCollectedEventsParameters>(this.OnCubeCollected);
    }

    private void OnDisable()
    {
        EventBus.RemoveListener<ItemCollectedEventsParameters>(this.OnItemCollected);
        EventBus.RemoveListener<CubeCollectedEventsParameters>(this.OnCubeCollected);
    }

    private void UpdateCoinsCounter()
    {
        if (this.maxCoinsCounter > 0)
        {
            this.coinsCounterElement.text = $"{this.coinsCounter} / {this.maxCoinsCounter}";
        }
        else
        {
            this.coinsCounterElement.text = this.coinsCounter.ToString();
        }
    }

    private void OnItemCollected(object sender, ItemCollectedEventsParameters parameters)
    {
        this.coinsCounter++;
        this.UpdateCoinsCounter();
    }

    private void OnCubeCollected(object sender, CubeCollectedEventsParameters parameters)
    {
        if (this.document.rootVisualElement.TryGet<VisualElement>(parameters.Type.ToString(), out var cube, true))
        {
            foreach (var item in this.sprites)
            {
                if (item.Type != parameters.Type)
                {
                    cube.style.backgroundImage = Background.FromSprite(item.Sprite);
                }
            }
        }
    }

    public enum CubeType
    {
        Red,
        Green,
        Blue
    }

    [Serializable]
    public struct CubeDef
    {
        public CubeType Type;
        public Sprite Sprite;
    }

    public struct CubeCollectedEventsParameters
    {
        public CubeType Type;
    }

    public struct ItemCollectedEventsParameters
    {
    }
}