using System.Collections;
using UnityEngine;

namespace OCSFX.EZFMOD.Utility
{
    /** <summary>
     * A MonoBehaviour that runs a Coroutine.
     * Optionally destroys itself when the Coroutine is finished.
     * </summary>
     */
    public class CoroutineRunner: MonoBehaviour
    {
        private Coroutine _currentRoutine;
        
        /** <summary>
         * Creates a new CoroutineRunner.
         * <param name="setName">The name of the new CoroutineRunner GameObject.</param>
         * </summary>
         */
        public static CoroutineRunner Create(string setName = null)
        {
            setName = string.IsNullOrWhiteSpace(setName) 
                ? $"{nameof(CoroutineRunner)}" : setName;
            
            var go = new GameObject(setName);
            var coroutineRunner = go.AddComponent<CoroutineRunner>();
            
            return coroutineRunner;
        }
        
        /** <summary>
         * Creates a new CoroutineRunner and runs the given routine.
         * <param name="routine">The Coroutine to run.</param>
         * <param name="destroyWhenFinished">Whether to destroy the runner when the Coroutine is finished. Default = true.</param>
         * </summary>
         */
        public static CoroutineRunner CreateAndRun(IEnumerator routine, bool destroyWhenFinished = true)
        {
            var coroutineRunner = Create();
            coroutineRunner.Run(routine, destroyWhenFinished);
            
            return coroutineRunner;
        }
         
        /** <summary>
         * Runs the given routine.
         * <param name="routine">The Coroutine to run.</param>
         * <param name="destroyWhenFinished">Whether to destroy the runner when the Coroutine is finished. Default = true.</param>
         * </summary>
         */
        public Coroutine Run(IEnumerator routine, bool destroyWhenFinished = true)
        {
            _currentRoutine = StartCoroutine(routine);
            if (destroyWhenFinished) StartCoroutine(Co_DestroyWhenFinished());
            
            return _currentRoutine;
        }
        
        public CoroutineRunner SetName(string newName)
        {
            gameObject.name = newName;
            return this;
        }
        
        public CoroutineRunner SetHideFlags(HideFlags flags)
        {
            gameObject.hideFlags = flags;
            return this;
        }
        
        public CoroutineRunner SetDontDestroyOnLoad()
        {
            DontDestroyOnLoad(gameObject);
            return this;
        }
        
        public CoroutineRunner AttachTo(Transform parent)
        {
            transform.SetParent(parent);
            return this;
        }

        private IEnumerator Co_DestroyWhenFinished()
        {
            yield return _currentRoutine;
            Destroy(gameObject);
        }
    }   
}
