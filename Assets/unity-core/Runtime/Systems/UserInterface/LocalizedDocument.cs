using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LocalizedDocument : MonoBehaviour
{
    [SerializeField]
    private UIDocument document;

    private void OnEnable()
    {
        EventBus.AddListener<LanguageChangedEventParameters>(this.OnLanguageChanged);
    }

    private void OnDisable()
    {
        EventBus.RemoveListener<LanguageChangedEventParameters>(this.OnLanguageChanged);
    }

    private void OnLanguageChanged(object sender, LanguageChangedEventParameters parameters)
    {
        this.document?.rootVisualElement?.Localize(parameters.Translations);
    }
}
