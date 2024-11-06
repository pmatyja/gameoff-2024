using FMODUnity;
using UnityEngine;

namespace Runtime.Interactions
{
    [CreateAssetMenu(menuName = GameOff2024Statics.MENU_ROOT + nameof(CollectableData))]
    public class CollectableData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        
        [field: Header("Effects")]
        [field: SerializeField] public GameObject OnCollectVfx { get; private set; }
        [field: SerializeField] public float OnCollectVfxLifetime { get; private set; } = 2f;
        [field: SerializeField] public EventReference OnCollectSfx { get; private set; }

        public void OnCollect(Transform collectableTransform)
        {
            if (OnCollectVfx)
            {
                var vfxInstance = Instantiate(OnCollectVfx, collectableTransform.position, collectableTransform.rotation);
                Destroy(vfxInstance, OnCollectVfxLifetime);
            }

            if (!OnCollectSfx.IsNull)
            {
                RuntimeManager.PlayOneShot(OnCollectSfx, collectableTransform.position);
            }
        }
    }
}