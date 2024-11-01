using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nodes.Actions.Conversation;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
[DisallowMultipleComponent]
public abstract class DialogSystem : UiElement
{
    protected abstract ChoiceRef SelectedChoice { get; set; }
    protected bool CanPickChoice => this?.SelectedChoice?.CanPickChoice() ?? false;

    [SerializeField]
    [InputActionMap]
    private string inputUp;

    [SerializeField]
    [InputActionMap]
    private string inputDown;

    [SerializeField]
    [InputActionMap]
    private string inputLeft;

    [SerializeField]
    [InputActionMap]
    private string inputRight;

    [SerializeField]
    [InputActionMap]
    private string inputAdvance;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float delayBeforeInput = 0.25f;

    private const float MaxDialogLength = 300.0f;

    private Coroutine coroutine;
    private bool inConversation;
    private bool isChoiceConfirmed = false;

    public IEnumerator PlayConversation(ConversationNode conversation, IEnumerable<Choice> choices, CoroutineResult<ChoiceRef> result)
    {
        if (this.inConversation)
        {
            Debug.LogWarning("Conversation is already playing");
            yield break;
        }

        GameManager.Mode = GameMode.Cutscene;

        yield return this.OnConversation(conversation, choices, result);
    }

    public override bool IsVisible(GameMode mode)
    {
        return mode == GameMode.Cutscene || mode == GameMode.Settings;
    }

    public override void Enable(MonoBehaviour behaviour, VisualElement root)
    {
        base.Enable(behaviour, root);
    }

    public override void Disable()
    {
        base.Disable();
    }

    protected abstract IEnumerator OnShow(GameObject anchor, ActorSO actor, string text, IEnumerable<SpriteDefinition> sprites, params ChoiceRef[] choices);
    protected abstract IEnumerator OnHide();

    protected bool ConfirmChoice()
    {
        if (this.SelectedChoice == null || this.SelectedChoice.CanPickChoice())
        {
            this.isChoiceConfirmed = true;
            return true;
        }
        
        return false;
    }

    protected virtual void NavigateUp()
    {
    }

    protected virtual void NavigateDown()
    {
    }

    protected virtual void NavigateLeft()
    {
    }

    protected virtual void NavigateRight()
    {
    }

    private IEnumerator OnConversation(ConversationNode conversation, IEnumerable<Choice> choices, CoroutineResult<ChoiceRef> result)
    {
        this.inConversation = true;

        var index = 0;
        var availableChoices = choices
            .Select(choice => new ChoiceRef(index++, choice))
            .Where(choice => choice.IsVisible)
            .ToArray();

        yield return this.OnShow(conversation.Anchor, conversation.Actor, conversation.Text, conversation.Sprites, availableChoices);
        yield return Wait.Seconds(this.delayBeforeInput);

        var displayLength = float.MaxValue;
        var audioLength = Mathf.Max(UITheme.GetTextDisplayTime(conversation.Text));

        if (conversation.Progression == ConversationProgression.Automatic)
        {
            if (availableChoices.Any() == false)
            {
                displayLength = audioLength;
            }
        }

        var audioSource = default(IAudioSource);

        if (conversation.VoiceLine != null)
        {
            audioSource = conversation.VoiceLine.Play(conversation.Anchor);

            if (audioSource != null)
            {
                audioLength = audioSource.Duration;
            }
        }
        else if (conversation.Actor?.Voice != null)
        {
            audioSource = conversation.Actor?.Voice.Play(conversation.Anchor);
        }

        this.isChoiceConfirmed = false;

        yield return new WaitUntil(() =>
        {
            if (InputManager.Released(this.inputUp))
            {
                this.NavigateUp();
            }

            if (InputManager.Released(this.inputDown))
            {
                this.NavigateDown();
            }

            if (InputManager.Released(this.inputLeft))
            {
                this.NavigateLeft();
            }

            if (InputManager.Released(this.inputRight))
            {
                this.NavigateRight();
            }

            if (InputManager.Released(this.inputAdvance))
            {
                return this.ConfirmChoice();
            }

            if (this.isChoiceConfirmed)
            {
                return true;
            }

            audioLength -= Time.deltaTime;

            if (audioLength < 0.0f)
            {
                audioSource?.Stop();
            }

            displayLength -= Time.deltaTime;

            return displayLength <= 0.0f;
        });

        audioSource?.Stop();

        Logger.Developer($"Choice: {this.SelectedChoice?.Text} picked");

        result.SetResult(this.SelectedChoice);

        yield return this.OnHide();

        this.inConversation = false;
    }
}