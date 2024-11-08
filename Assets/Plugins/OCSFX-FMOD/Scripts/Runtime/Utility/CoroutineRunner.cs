using System.Collections;
using UnityEngine;

namespace OCSFX.Utility
{
    public class CoroutineRunner: MonoBehaviour
    {
        // Monobehaviour simply to run a couroutine.
        // Optionally destroys itself when the routine is finished.
        private Coroutine _currentRoutine;

        public static CoroutineRunner Create()
        {
            var go = new GameObject($"{nameof(CoroutineRunner)}");
            var holder = go.AddComponent<CoroutineRunner>();
            
            return holder;
        }
        
        public CoroutineRunner SetName(string newName)
        {
            gameObject.name = newName;
            return this;
        }
        
        public static CoroutineRunner CreateAndRun(IEnumerator routine, bool destroyWhenFinished = true)
        {
            var go = new GameObject($"{nameof(CoroutineRunner)}");
            var holder = go.AddComponent<CoroutineRunner>();
            holder.Run(routine, destroyWhenFinished);
            
            return holder;
        }
            
        public CoroutineRunner Run(IEnumerator routine, bool destroyWhenFinished = true)
        {
            _currentRoutine = StartCoroutine(routine);
            if (destroyWhenFinished) StartCoroutine(Co_DestroyWhenFinished());
            
            return this;
        }

        private IEnumerator Co_DestroyWhenFinished()
        {
            yield return _currentRoutine;
            Destroy(gameObject);
        }
    }   
}
