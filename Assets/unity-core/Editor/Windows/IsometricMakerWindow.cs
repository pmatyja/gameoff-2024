using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class IsometricMakerWindow : EditorWindow
{
    private VisualElement content;

    public IsometricMakerWindow()
    {
        this.titleContent = new GUIContent("Isometric Maker");
    }

    [MenuItem("Lavgine/Isometric Maker")]
    private static void Init()
    {
        var window = GetWindow<IsometricMakerWindow>();

        window.ShowTab();
        window.Focus();
    }

    private void OnEnable()
    {
        this.OnDisable();

        this.content = new VisualElement
        {
            style =
            {
                paddingTop = new StyleLength(new Length(3.0f, LengthUnit.Pixel)),
                paddingBottom = new StyleLength(new Length(8.0f, LengthUnit.Pixel)),
                paddingLeft = new StyleLength(new Length(8.0f, LengthUnit.Pixel)),
                paddingRight = new StyleLength(new Length(8.0f, LengthUnit.Pixel)),
            }
        };

        this.rootVisualElement.Add(this.content);
        this.content.Add(new PropertyField());

        this.content.StretchToParentSize();
    }

    private void OnDisable()
    {
        this.content?.RemoveFromHierarchy();
        this.content = null;
    }
}
