using UnityEngine.UIElements;

public static class LocalizationExtensions
{
    public static string Localize(this string text, TranslationsSO translations = null)
    {
        translations ??= GameManager.Instance.Translations;

        if (translations == null)
        {
            return text;
        }

        return translations?.Format(text) ?? text;
    }

    public static void Localize(this VisualElement element)
    {
        element.Localize(GameManager.Instance.Translations);
    }

    public static void Localize(this VisualElement element, TranslationsSO translations)
    {
        if (translations == null)
        {
            return;
        }

        if (element is UnityEngine.UIElements.TextElement textElement)
        {
            textElement.userData = textElement.text;
            textElement.text = translations?.Format(textElement.userData as string) ?? textElement.text;
        }

        var elementHierarchy = element.hierarchy;

        for (int i = 0; i < elementHierarchy.childCount; i++)
        {
            Localize(elementHierarchy.ElementAt(i), translations);
        }
    }
}