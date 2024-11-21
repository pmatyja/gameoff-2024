using UnityEngine;
using UnityEngine.UIElements;

public class UIHoverDetector : Singleton<UIHoverDetector>
{
    public static bool Hovering => Element != default;
    public static VisualElement Element;

    [SerializeField]
    [Readonly]
    private string element;

    private void Start()
    {
        var items = GameObject.FindObjectsByType<UIDocument>(FindObjectsSortMode.None);

        foreach (var item in items)
        {
            item.rootVisualElement.RegisterCallback<PointerOverEvent>(this.OnEnter, TrickleDown.TrickleDown);
            item.rootVisualElement.RegisterCallback<PointerOutEvent>(this.OnLeave, TrickleDown.TrickleDown);
        }
    }

    private void OnEnter(PointerOverEvent evt)
    {
        var element = evt.target as VisualElement;
        if (element == default)
        {
            return;
        }

        if (element.pickingMode == PickingMode.Ignore)
        {
            return;
        }

        if (element.style.display == DisplayStyle.None)
        {
            return;
        }

        Element = element;

        this.element = $"{Element.GetType().Name}: '{Element.name}'";
    }

    private void OnLeave(PointerOutEvent evt)
    {
        if (Element == evt.target as VisualElement)
        {
            Element = default;
            this.element = default;
        }
    }
}
