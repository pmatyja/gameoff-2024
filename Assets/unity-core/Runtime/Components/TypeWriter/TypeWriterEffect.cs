using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TypeWriterEffect
{
    [SerializeField]
    public EventChannelSO Channel;

    [SerializeField]
    [Readonly]
    private bool isAnimating;
    public bool IsAnimating => this.index < this.source.Length;

    [SerializeField]
    [HideLabel]
    [Multiline(10)]
    private string source;
    public string Source
    {
        get => this.source;
        set
        {
            this.source = value;
            this.index = 0;
            this.time = 0.0f;
            this.isAnimating = this.index < (value?.Length ?? 0);
            this.closeCodes.Clear();
        }
    }

    [SerializeField]
    [Range(0f, 1f)]
    private float delay = 0.05f;

    [SerializeField]
    [HideLabel]
    [Readonly]
    [Multiline(10)]
    private string text;
    public string Text => this.text;

    [SerializeField]
    [Readonly]
    private int index;

    [SerializeField]
    [Readonly]
    [Range(0f, 1f)]
    private float time;

    private List<string> closeCodes = new();

    public void Advance()
    {
        if (string.IsNullOrWhiteSpace(this.source))
        {
            this.isAnimating = false;
            return;
        }

        this.isAnimating = this.index < this.source.Length;

        if (this.index >= this.source.Length)
        {
            return;
        }

        this.time += Time.deltaTime;

        if (this.time < this.delay)
        {
            return;
        }

        var character = this.ReadCharacter();
        if (character == '<')
        {
            this.HandleCode();
        }

        this.text = this.source?.Substring(0, this.index) + string.Join(' ', this.closeCodes);
        this.time = 0.0f;
        
        if (this.index == 0)
        {
            EventBus.Raise(this, this.Channel, new TypeWriterEventParameters
            {
                State = TypeWriterState.Start,
                Character = character,
                Text = this.text,
                Index = this.index
            });
        }
        else if (this.index >= this.source.Length)
        {
            EventBus.Raise(this, this.Channel, new TypeWriterEventParameters
            {
                State = TypeWriterState.End,
                Character = character,
                Text = this.text,
                Index = this.index
            });
        }
        else
        {
            EventBus.Raise(this, this.Channel, new TypeWriterEventParameters
            {
                State = TypeWriterState.InProgress,
                Character = character,
                Text = this.text,
                Index = this.index
            });
        }
    }

    private char ReadCharacter()
    {
        while (this.index < this.source.Length)
        {
            var character = this.source[this.index];

            // skip characters

            if (character == '\\' || character == '\r')
            {
                this.index++;
                continue;
            }

            this.index++;

            return character;
        }

        return char.MinValue;
    }

    private string ReadUntil(Func<char, bool> onPredicate)
    {
        var start = this.index;

        while (this.index < this.source.Length)
        {
            var character = this.ReadCharacter();
            if (onPredicate.Invoke(character) == false)
            {
                return this.source.Substring(start, this.index - start - 1);
            }
        }

        return this.source.Substring(start, this.index - 1);
    }

    private void HandleCode()
    {
        var sign = this.source[this.index];
        if (sign == '/')
        {
            this.index++;
        }

        var code = this.ReadUntil(c => c != '>');
        var name = string.Empty;

        for (var i = 0; i < code.Length; i++)
        {
            var character = code[i];

            if (char.IsLetter(character) == false)
            {
                break;
            }

            name += character;
        }

        if (sign == '/')
        {
            closeCodes.Remove($"</{name}>");
        }
        else
        {
            closeCodes.Add($"</{name}>");
        }
    }
}
