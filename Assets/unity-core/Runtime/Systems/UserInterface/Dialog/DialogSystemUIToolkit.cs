using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

[Serializable]
public class DialogSystemUIToolkit : DialogSystem
{
    protected override ChoiceRef SelectedChoice { get; set; }

    [SerializeField]
    [NotNull("File 'Shared/VTA_Gameplay_DialogSystem_Choice.uxml' not found")]
    [Readonly]
    private VisualTreeAsset template;

    private VisualElement spritesElement;
    private VisualElement choicesElement;

    private Label contentElement;

    private VisualElement currentChoice;

    public override void Enable(MonoBehaviour behaviour, VisualElement root)
    {
        base.Enable(behaviour, root.Q<VisualElement>(nameof(DialogSystem)));

        this.Root.SetOpacity(0.0f);

        this.spritesElement = this.Root.Q<VisualElement>("Sprites");
        this.spritesElement.Clear();

        this.choicesElement = this.Root.Q<VisualElement>("Choices");
        this.choicesElement.Clear();

        this.contentElement = this.Root.Q<Label>("Content");
    }

    public override void OnValidate()
    {
        base.OnValidate();

        EditorOnly.LoadAsset("Assets/Resources/Shared/VTA_Gameplay_DialogSystem_Choice.uxml", out this.template);
    }

    protected override IEnumerator OnShow(GameObject anchor, ActorSO actor, string text, IEnumerable<SpriteDefinition> sprites, params ChoiceRef[] choices)
    {
        yield return this.OnHide();

        if (actor != null)
        {
            this.contentElement.text = $"<b><color=#{ColorUtility.ToHtmlStringRGB(actor.Color)}>{actor.Name}</color></b>: ";
        }
        else
        {
            this.contentElement.text = "";
        }

        this.contentElement.text += text;

        var hasSelected = false;
        var elementIndex = 0;

        if (this.spritesElement != null)
        {
            this.spritesElement.Clear();

            foreach (var sprite in sprites)
            {
                if (sprite.Sprite == null)
                {
                    continue;
                }

                var child = new VisualElement();

                child.style.width = new StyleLength(new Length(sprite.Sprite.texture.width, LengthUnit.Pixel));
                child.style.height = new StyleLength(new Length(sprite.Sprite.texture.height, LengthUnit.Pixel));
                child.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(BackgroundSizeType.Contain));
                child.style.backgroundImage = Background.FromSprite(sprite.Sprite);
                child.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);

                if (sprite.FlipHorizontally)
                {
                    child.transform.scale = new Vector3(-1.0f, 1.0f, 1.0f);
                }

                this.spritesElement.Add(child);
            }
        }

        if (choices != null && choices.Any())
        {
            foreach (var choice in choices)
            {
                var element = this.template.Instantiate().Children().First();
                
                element.userData = choice;

                if (element.Q<Label>("Content") is Label content)
                {
                    if (choice.IsBlocked)
                    {
                        content.text = $"\ue897 {choice.Text}";
                    }
                    else
                    {
                        content.text = $"\ue836 {choice.Text}";
                    }
                }

                if (choice.IsPrimary)
                {
                    element.AddToClassList("choice-primary");
                }

                if (choice.IsBlocked)
                {
                    element.AddToClassList("choice-locked");
                }
                else
                {
                    element.OnClick(evt =>
                    {
                        this.SelectedChoice = choice;
                        this.ConfirmChoice();

                        EventBus.Raise<MouseClickEvent>(this, new MouseClickEvent
                        {
                            Element = element
                        });
                    });
                }

                element.OnHover(evt =>
                {
                    this.SelectedChoice = choice;
                    this.NavigateTo(this.choicesElement.IndexOf(element));

                    EventBus.Raise<MouseHoverEvent>(this, new MouseHoverEvent
                    {
                        Element = element
                    });
                });

                if (choice.IsBlocked == false && hasSelected == false)
                {
                    hasSelected = true;

                    this.currentChoice = element;
                    element.AddToClassList("choice-selected");
                }

                elementIndex++;

                this.choicesElement.Add(element);
            }
        }

        this.choicesElement.SetOpacity(this.choicesElement.childCount > 0 ? 1.0f : 0.0f);

        this.contentElement.Localize();
        this.choicesElement.Localize();

        if (choices == null || choices.Length < 1)
        {
            this.Root.OnClick(evt =>
            {
                this.ConfirmChoice();
            });
        }

        yield return this.Root.FadeIn();
    }

    protected override IEnumerator OnHide()
    {
        yield return this.Root.FadeOut();
        yield return Wait.Seconds(0.25f);

        this.choicesElement.Clear();
    }

    protected override void NavigateDown()
    {
        var currentIndex = this.choicesElement.IndexOf(this.currentChoice);
        var newIndex = currentIndex;

        newIndex = Mathf.Min(newIndex + 2, this.choicesElement.childCount - 1);

        if (newIndex != currentIndex)
        {
            this.NavigateTo(newIndex);
        }
    }

    protected override void NavigateLeft()
    {
        var currentIndex = this.choicesElement.IndexOf(this.currentChoice);
        var newIndex = currentIndex;

        if (newIndex % 2 == 1)
        {
            newIndex = Mathf.Max(newIndex - 1, 0);
        }

        if (newIndex != currentIndex)
        {
            this.NavigateTo(newIndex);
        }
    }

    protected override void NavigateRight()
    {
        var currentIndex = this.choicesElement.IndexOf(this.currentChoice);
        var newIndex = currentIndex;

        if (newIndex % 2 == 0)
        {
            newIndex = Mathf.Min(newIndex + 1, this.choicesElement.childCount - 1);
        }

        if (newIndex != currentIndex)
        {
            this.NavigateTo(newIndex);
        }
    }

    protected override void NavigateUp()
    {
        var currentIndex = this.choicesElement.IndexOf(this.currentChoice);
        var newIndex = currentIndex;

        if (newIndex > 1)
        {
            newIndex = Mathf.Max(newIndex - 2, 0);
        }

        if (newIndex != currentIndex)
        {
            this.NavigateTo(newIndex);
        }
    }

    private void NavigateTo(int newIndex)
    {
        this.currentChoice?.RemoveFromClassList("choice-selected");
        this.currentChoice = this.choicesElement[newIndex];
        this.currentChoice?.AddToClassList("choice-selected");
    }
}