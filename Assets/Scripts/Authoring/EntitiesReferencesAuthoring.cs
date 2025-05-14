using Unity.Entities;
using UnityEngine;

class EntitiesReferencesAuthoring : MonoBehaviour
{
    [SerializeField]
    private GameObject nikkePrefab;
    
    class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new EntitiesReferences
            {
                nikkePrefabEntity = GetEntity(authoring.nikkePrefab, TransformUsageFlags.Dynamic),
            });
        }
    }    
}

public struct EntitiesReferences : IComponentData
{
    public Entity nikkePrefabEntity;
}