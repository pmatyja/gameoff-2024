using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ChoiceTrackerWindow : EditorWindow
{
    private Toolbar toolbar;
    private VisualElement content;
    private ChoiceTrackerSO instance;

    public ChoiceTrackerWindow()
    {
        this.titleContent = new GUIContent("Choice Tracker");
    }

    [MenuItem("Lavgine/Choice Tracker")]
    private static void Init()
    {
        var window = GetWindow<ChoiceTrackerWindow>();

        window.ShowTab();
        window.Focus();
    }

    private void OnEnable()
    {
        this.OnDisable();

        this.toolbar = new Toolbar
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                flexGrow = 0,
                backgroundColor = new Color(0.25f, 0.25f, 0.25f)
            }
        };

        var button = new Button();
        {
            button.text = "Reload";
            button.OnClick(_ => this.LoadData());

            toolbar.Add(button);
        }

        var reset = new Button();
        {
            reset.text = "Reset values";
            reset.OnClick(_ => this.instance?.Reset());

            toolbar.Add(reset);
        }

        var findAsset = new Button();
        {
            findAsset.text = "Find Asset";
            findAsset.OnClick(_ =>
            {
                if (this.instance != null)
                {
                    EditorGUIUtility.PingObject(this.instance.GetInstanceID());
                    Selection.activeInstanceID = this.instance.GetInstanceID();
                }
            });

            toolbar.Add(findAsset);
        }

        {
            var trackers = EditorOnly.FindAssets<ChoiceTrackerSO>().Select(x => x.Replace("/", "\\")).ToList();
            var dropdown = new DropdownField(trackers, 0);

            if (this.instance != null)
            {
                dropdown.index = trackers.FindIndex(x => string.Equals(x, EditorOnly.GetAssetPath(this.instance), StringComparison.OrdinalIgnoreCase));
            }

            dropdown.RegisterValueChangedCallback<string>(evt =>
            {
                EditorOnly.LoadAsset<ChoiceTrackerSO>(dropdown.value, out this.instance);

                if (this.instance == null)
                {
                    dropdown.choices = EditorOnly.FindAssets<ChoiceTrackerSO>().Select(x => x.Replace("/", "\\")).ToList();
                    dropdown.index = 0;

                    if (dropdown.choices.Any())
                    {
                        EditorOnly.LoadAsset<ChoiceTrackerSO>(dropdown.value, out this.instance);
                    }
                }

                this.LoadData();
            });
            
            toolbar.Add(dropdown);
        }

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
        this.rootVisualElement.Add(this.toolbar);

        this.content.StretchToParentSize();
        this.toolbar.BringToFront();

        this.LoadData();
    }

    private void OnDisable()
    {
        this.toolbar?.RemoveFromHierarchy();
        this.toolbar = null;

        this.content?.RemoveFromHierarchy();
        this.content = null;
    }

    private void LoadData()
    {
        this.content.Clear();

        if (this.instance == null)
        {
            return;
        }

        var serializedObject = new SerializedObject(this.instance);
        var serializedProperty = serializedObject.FindProperty(nameof(ChoiceTrackerSO.Choices));

        serializedProperty.isExpanded = true;

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        var root = new PropertyField();

        root.BindProperty(serializedProperty);

        this.content.Add(root);
    }
}
