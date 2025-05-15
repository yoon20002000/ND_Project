using Unity.Entities;
using UnityEngine;

class EntitiesReferencesAuthoring : MonoBehaviour
{
    [SerializeField]
    private GameObject nikkePrefab;
    [SerializeField]
    private GameObject hitParticle;
    class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new EntitiesReferences
            {
                NikkePrefabEntity = GetEntity(authoring.nikkePrefab, TransformUsageFlags.Dynamic),
                HitParticleEntity = GetEntity(authoring.hitParticle, TransformUsageFlags.Dynamic),
            });
        }
    }    
}

public struct EntitiesReferences : IComponentData
{
    public Entity NikkePrefabEntity;
    public Entity HitParticleEntity;
}