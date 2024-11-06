using System;
using UnityEngine;

namespace Lavgine
{
    public class Wobble : MonoBehaviour
    {
        [SerializeField]
        private TweenCurve tween = new()
        {
            PingPong = true
        };

        [SerializeField]
        [Range(0.01f, 2.0f)]
        private float distance = 0.5f;

        [SerializeField]
        [Range(0.001f, 10.0f)]
        private float duration = 0.125f;

        private void Update()
        {
            this.tween.Update();
            this.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0, this.distance, 0), this.tween.Value);
        }
    }
}
