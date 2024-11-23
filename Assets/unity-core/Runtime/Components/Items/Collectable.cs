using System;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeReference]
    private EventChannelSO channel;

    [SerializeField]
    private LayerMask picker;
    
    [SerializeField]
    private bool showDebug;

    protected virtual void Start()
    {
        OnSpawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!this.IsValidPicker(collision.gameObject))
        {
            return;
        }
        
        this.OnPickUp(collision.gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!this.IsValidPicker(collision.gameObject))
        {
            return;
        }
        
        this.OnPickUp(collision.gameObject);
    }

    protected virtual void OnPickUp(GameObject pickerObject)
    {
        EventBus.Raise(this, this.channel, new CollectableEventParameters
        {
            Picker = pickerObject,
            Collectable = this
        });
        
        if (this.showDebug)
        {
            Debug.Log($"{pickerObject.name} picked up {this.gameObject.name}" +
                      $"\nEvent channel: {(this.channel ? this.channel.name : "null")}");
        }

        GameObject.Destroy(gameObject);
    }
    
    protected virtual void OnSpawn()
    {
    }
    
    private bool IsValidPicker(GameObject pickerObject) => picker.ContainsLayer(pickerObject.layer);
}
