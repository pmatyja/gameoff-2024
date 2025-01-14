using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Interactions
{
    public class SimplePrefabSpawner : MonoBehaviour
    {
        [SerializeField] protected GameObject _prefab;
        [SerializeField] protected Transform _defaultSpawnPoint;

        [Header("Settings")]
        [SerializeField] protected bool _spawnOnStart;
        [SerializeField] protected bool _useLifeTime;
        [SerializeField] protected float _lifeTime = 2f;
        
        private readonly HashSet<GameObject> _instances = new HashSet<GameObject>();

        private void Awake()
        {
            if (!_defaultSpawnPoint) _defaultSpawnPoint = transform;
        }
        
        private void Start()
        {
            if (_spawnOnStart) Spawn();
        }

        public void Spawn()
        {
            if (!_defaultSpawnPoint) _defaultSpawnPoint = transform;
            
            SpawnAt(_defaultSpawnPoint.position, _defaultSpawnPoint.rotation);
        }
        
        public void Spawn(Transform spawnPoint)
        {
            SpawnAt(spawnPoint.position, spawnPoint.rotation);
        }
        
        public void Spawn(Vector3 position)
        {
            SpawnAt(position);
        }
        
        public void Spawn(Vector3 position, Quaternion rotation)
        {
            SpawnAt(position, rotation);
        }
        
        protected void SpawnAt(Vector3 position, Quaternion rotation = default)
        {
            if (!_prefab) return;
            
            CleanInstances();
            
            var instance = Instantiate(_prefab, position, rotation);

            if (_lifeTime <= 0 || !_useLifeTime) return;
            
            StartCoroutine(Co_HandleLifetime(instance));

            _instances.Add(instance);
        }

        private IEnumerator Co_HandleLifetime(GameObject instance)
        {
            var timeAlive = 0f;
            
            while (timeAlive < _lifeTime)
            {
                timeAlive += Time.deltaTime;
                yield return null;
            }
            
            CleanInstances();
            
            if (!instance) yield break;
            
            _instances.Remove(instance);
            Destroy(instance);
        }

        public GameObject[] GetInstances()
        {
            return _instances.ToArray();
        }
        
        public void DestroyInstance(GameObject instance)
        {
            if (!_instances.Contains(instance)) return;
            
            _instances.Remove(instance);
            Destroy(instance);
        }
        
        public void DestroyInstances()
        {
            CleanInstances();
            
            foreach (var instance in _instances)
            {
                _instances.Remove(instance);
                Destroy(instance);
            }
        }

        private void CleanInstances()
        {
            _instances.RemoveWhere(existing => !existing);
        }
    }
}