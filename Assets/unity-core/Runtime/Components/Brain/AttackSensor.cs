using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackSensor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //EventBus.Raise<AttackEventParameters>(this.gameObject, default);
    }
}

[Serializable]
public struct AttackEventParameters
{

}