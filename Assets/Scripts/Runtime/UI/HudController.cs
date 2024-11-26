using System;
using Runtime;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HudController : Singleton<HudController>
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

    [SerializeField]
    private TweenCurve tween = new TweenCurve();

    private float shardsProgress = 1.0f;
    private VisualElement shardsElement;

    private Label coinsCounterElement;

    private void Start()
    {
        this.document = this.GetComponent<UIDocument>();

        this.document.rootVisualElement.TryGet("Shards", out this.shardsElement, true);
        this.document.rootVisualElement.TryGet("Coins", out this.coinsCounterElement, true);

        this.maxCoinsCounter = GameOff2024Statics.GetOptionalCollectableTotal();

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
        
        GameOff2024Statics.OnOptionalCollectableTotalChanged += this.UpdateMaxCoinCount;
    }

    private void OnDisable()
    {
        EventBus.RemoveListener<ItemCollectedEventsParameters>(this.OnItemCollected);
        EventBus.RemoveListener<CubeCollectedEventsParameters>(this.OnCubeCollected);
        
        GameOff2024Statics.OnOptionalCollectableTotalChanged -= this.UpdateMaxCoinCount;
    }

    private void Update()
    {
        if (this.shardsElement != default)
        {
            this.shardsProgress = Tween.Interpolate(this.shardsProgress, this.tween.Duration, tween.PingPong, 1.0f, value =>
            {
                var x = this.tween.GetValue(this.shardsProgress);
                var y = this.tween.GetValue(this.shardsProgress);

                this.shardsElement.style.scale = new StyleScale(new Vector2(x, y));
            });
        }
    }
    
    private void UpdateMaxCoinCount(int total)
    {
        this.maxCoinsCounter = total;
        this.UpdateCoinsCounter();
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
        this.shardsProgress = 0.0f;
        this.coinsCounter++;
        this.UpdateCoinsCounter();
    }

    private void OnCubeCollected(object sender, CubeCollectedEventsParameters parameters)
    {
        if (this.document.rootVisualElement.TryGet<VisualElement>(parameters.Type.ToString(), out var cube, true))
        {
            foreach (var item in this.sprites)
            {
                if (item.Type == parameters.Type)
                {
                    cube.style.backgroundImage = Background.FromSprite(item.Sprite);
                }
            }
        }
    }

    private void SimulateCube(CubeType cube)
    {
        EventBus.Raise(this, new CubeCollectedEventsParameters
        {
            Type = cube
        });
    }

    [ContextMenu("Test: Red Cube")] private void TestRedCube() { this.SimulateCube(CubeType.Red); }
    [ContextMenu("Test: Green Cube")] private void TestGreenCube() { this.SimulateCube(CubeType.Green); }
    [ContextMenu("Test: Blue Cube")] private void TestBlueCube() { this.SimulateCube(CubeType.Blue); }

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