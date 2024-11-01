using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementExtensions
{
    public static readonly Color BackgroundColor = new Color(0.33f, 0.33f, 0.33f, 1.0f);
    public static readonly Color HoverBackgrounColor = new Color(0.53f, 0.53f, 0.53f, 1.0f);
    public static readonly Color ClickBackgroundColor = new Color(70.0f / 255.0f, 96.0f / 255.0f, 124.0f / 255.0f, 1.0f);
    public static readonly Color SeparatorColor = new Color(0.21f, 0.21f, 0.21f, 1.0f);

    public static VisualElement AddChild<T>(this VisualElement root, Action<T> action = null) where T : VisualElement, new()
    {
        var element = new T();
        root.Add(element);
        action?.Invoke(element);
        return element;
    }

    public static bool IsEnabled(this VisualElement element)
    {
        if (element.enabledSelf == false)
        {
            return false;
        }

        var elem = element.parent;

        while (elem != null)
        {
            if (elem.enabledSelf == false)
            {
                return false;
            }

            elem = elem.parent;
        }

        return true;
    }

    public static bool IsVisible(this VisualElement element)
    {
        if (element.visible == false)
        {
            return false;
        }

        var elem = element.parent;

        while (elem != null)
        {
            if (elem.visible == false)
            {
                return false;
            }

            elem = elem.parent;
        }

        return true;
    }

    public static bool DoesMatch<T>(this VisualElement element, Func<T, bool> predicate = null) where T : VisualElement
    {
        if (element is T asType)
        {
            if (predicate == null)
            {
                return true;
            }

            return predicate(asType);
        }

        return false;
    }

    public static void DisablePicking(this VisualElement element, bool recursive = true)
    {
        if (element == null)
        {
            return;
        }

        element.pickingMode = PickingMode.Ignore;

        foreach (var child in element.Children())
        {
            DisablePicking(child, recursive);
        }
    }

    public static void SetOpacity(this VisualElement element, float opacity)
    {
        element.style.opacity = opacity;

        if (opacity > 0.001f)
        {
            element.SetEnabled(true);
            element.visible = true;
            element.style.display = DisplayStyle.Flex;
        }

        if (opacity <= 0.001f)
        {
            element.SetEnabled(false);
            element.visible = false;
            element.style.display = DisplayStyle.None;
        }
    }

    public static void Fade(this VisualElement element, float target, float duration = UITheme.TransitionDuration)
    {
        element.SetOpacity(Tween.Once(element.style.opacity.value, duration, target));
    }

    public static IEnumerator FadeIn(this VisualElement element, float duration = UITheme.TransitionDuration)
    {
        yield return Tween.Once(element.style.opacity.value, duration, 1.0f, element.SetOpacity);
    }

    public static IEnumerator FadeOut(this VisualElement element, float duration = UITheme.TransitionDuration)
    {
        yield return Tween.Once(element.style.opacity.value, duration, 0.0f, element.SetOpacity);
    }

    public static IEnumerator OpenPage(this VisualElement content, VisualTreeAsset template)
    {
        content.Clear();
        template.CloneTree(content);
        content.SetOpacity(0.0f);

        yield return FadeIn(content);
    }

    public static bool TryGet<T>(this VisualElement parent, out T element, bool raiseWarningWhenNotFound = false) where T : VisualElement
    {
        if (parent == null)
        {
            if (raiseWarningWhenNotFound)
            {
                Debug.LogWarning($"Element '{typeof(T).Name}' not found in '{parent.name}'");
            }

            element = null;
            return false;
        }

        if (parent.Q<T>() is T child)
        {
            element = child;
            return true;
        }

        if (raiseWarningWhenNotFound)
        {
            Debug.LogWarning($"Element '{typeof(T).Name}' not found in '{parent.name}'");
        }

        element = default;
        return false;
    }

    public static bool TryGet<T>(this VisualElement parent, string id, out T element, bool raiseWarningWhenNotFound = false) where T : VisualElement
    {
        if ( parent == null )
        {
            if (raiseWarningWhenNotFound)
            {
                Debug.LogWarning($"Element '{id}' not found");
            }

            element = null;
            return false;
        }

        var found = parent.Q<T>(id);
        if (found != null)
        {
            element = found;
            return true;
        }

        foreach (var child in parent.Children())
        {
            if (child.TryGet<T>(id, out element, raiseWarningWhenNotFound))
            {
                return true;
            }
        }

        if (raiseWarningWhenNotFound)
        {
            Debug.LogWarning($"Element '{id}' not found in '{parent.name}'");
        }

        element = default;
        return false;
    }

    public static IEnumerable<T> GetChildren<T>(this VisualElement parent, bool recursive = true, Func<T, bool> predicate = null) where T : VisualElement
    {
        if (parent == null)
        {
            return Enumerable.Empty<T>();
        }

        var results = new List<T>();

        if (parent.DoesMatch<T>(predicate))
        {
            results.Add(parent as T);
        }

        foreach (var child in parent.Children())
        {
            if (recursive)
            {
                results.AddRange(child.GetChildren<T>(recursive, predicate));
            }
            else
            {
                if (child.DoesMatch<T>(predicate))
                {
                    results.Add(child as T);
                }
            }
        }

        return results;
    }

    public static void HoverChild(this VisualElement container, int index, string className)
    {
        foreach (var item in container.Children())
        {
            item.RemoveFromClassList(className);
        }
        
        if (index > -1 && index < container.childCount)
        {
            container.Children().Skip(index).FirstOrDefault()?.AddToClassList(className);
        }
    }

    public static void HoverChild(this VisualElement container, VisualElement element, string className)
    {
        foreach (var item in container.Children())
        {
            item.RemoveFromClassList(className);
        }
        
        element?.AddToClassList(className);
    }

    public static void OnEvent<T>(this VisualElement element, Action<T> onClick) where T : EventBase<T>, new()
    {
        element.RegisterCallback<T>(evt =>
        {
            if (element.IsEnabled())
            {
                onClick.Invoke(evt);
            }
        },
        TrickleDown.TrickleDown);
    }

    public static void OnClick(this VisualElement element, Action<MouseDownEvent> onClick)
    {
        element.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (element.IsEnabled())
            {
                onClick.Invoke(evt);
            }
        }, 
        TrickleDown.TrickleDown);
    }

    public static void OnHover(this VisualElement element, Action<MouseEnterEvent> onClick)
    {
        element.RegisterCallback<MouseEnterEvent>(evt =>
        {
            if (element.IsEnabled())
            {
                onClick.Invoke(evt);
            }
        },
        TrickleDown.TrickleDown);
    }

    public static void RegisterSlider(this VisualElement element, string id, EventCallback<ChangeEvent<float>> callback, float defaultValue = 1.0f)
    {
        if( element.TryGet<Slider>(id, out var slider) )
        {
            slider.RegisterValueChangedCallback(evt =>
            {
                if (element.IsEnabled())
                {
                    callback.Invoke(evt);
                }
            });

            slider.value = defaultValue;
        }
    }

    public static void RegisterSlider(this Slider element, EventCallback<ChangeEvent<float>> callback, float defaultValue = 1.0f)
    {
        element.RegisterValueChangedCallback(evt =>
        {
            if (element.IsEnabled())
            {
                callback.Invoke(evt);
            }
        });

        element.value = defaultValue;
    }

    public static void OnlyPC(this VisualElement content)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            content.Q<VisualElement>("Exit")?.RemoveFromHierarchy();
        }
    }

    public static VisualElement WithDisplay(this VisualElement element, bool visible)
    {
        if (visible)
        {
            element.style.display = DisplayStyle.Flex;
        }
        else
        {
            element.style.display = DisplayStyle.None;
        }

        return element;
    }

    public static VisualElement WithResponsiveFlex(this VisualElement element, float minWidth = 480.0f, Action<VisualElement> onColumn = null, Action<VisualElement> onRow = null)
    {
        if (element.contentRect.width < minWidth)
        {
            element.style.flexDirection = FlexDirection.Column;
            onColumn?.Invoke(element);
        }
        else
        {
            element.style.flexDirection = FlexDirection.Row;
            onRow?.Invoke(element);
        }

        return element;
    }
}
