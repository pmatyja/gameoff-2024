using DeBroglie;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeReference]
    private EventChannelSO channel;

    [SerializeField]
    private LayerMask picker;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.OnPickUp(collision.gameObject);
    }

    private void OnTriggerEnter3D(Collider collision)
    {
        this.OnPickUp(collision.gameObject);
    }

    private void OnPickUp(GameObject picker)
    {
        EventBus.Raise(this, this.channel, new CollectableEventParameters
        {
            Picker = picker,
            Collectable = this.gameObject
        });

        GameObject.Destroy(gameObject);
    }
}
