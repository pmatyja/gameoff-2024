using UnityEngine;
using UnityEngine.Events;

public class WakeUpEventListner : MonoBehaviour
{
    public EventChannelSO Channel;
    public UnityEvent Callback;

    private void OnEnable()
    {
        EventBus.AddListener<WakeUpEvent>(this.OnEventReceived, this.Channel);
    }

    private void OnDisable()
    {
        EventBus.RemoveListener<WakeUpEvent>(this.OnEventReceived);
    }

    private void OnEventReceived(object sender, WakeUpEvent parameters)
    {
        this.Callback?.Invoke();
    }
}
