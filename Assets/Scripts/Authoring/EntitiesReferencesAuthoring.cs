using Unity.Collections;
using Unity.Entities;
using UnityEngine;

class EntitiesReferencesAuthoring : MonoBehaviour
{
    [System.Serializable]
    public struct NikkePrefabEntry
    {
        public string NikkeName;
        public GameObject NikkePrefab;
    }
    
    [SerializeField] 
    private NikkePrefabEntry[] nikkePrefabs;
    [SerializeField]
    private GameObject hitParticle;
    class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);

            DynamicBuffer<NikkePrefabBuffer> nikkeBuffer = AddBuffer<NikkePrefabBuffer>(entity);

            for (int prefabIndex = 0; prefabIndex < authoring.nikkePrefabs.Length; ++prefabIndex)
            {
                NikkePrefabEntry nikkePrefabEntry = authoring.nikkePrefabs[prefabIndex];
                nikkeBuffer.Add(new NikkePrefabBuffer
                {
                    PrefabEntity = GetEntity(nikkePrefabEntry.NikkePrefab, TransformUsageFlags.Dynamic),
                    NikkeName = nikkePrefabEntry.NikkeName,
                });
            }
            
            AddComponent(entity, new EntitiesReferences
            {
                HitParticleEntity = GetEntity(authoring.hitParticle, TransformUsageFlags.Dynamic),
            });
        }
    }    
}

public struct EntitiesReferences : IComponentData
{
    public Entity HitParticleEntity;
}

public struct NikkePrefabBuffer : IBufferElementData
{
    public Entity PrefabEntity;
    public FixedString32Bytes NikkeName;
}