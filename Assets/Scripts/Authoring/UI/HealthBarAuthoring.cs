using Unity.Entities;
using UnityEngine;

class HealthBarAuthoring : MonoBehaviour
{
    public GameObject barVisualGameObject;
    public GameObject healthTargetGameObject;
    class Baker : Baker<HealthBarAuthoring>
    {
        public override void Bake(HealthBarAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new HealthBar
            {
                barVisualEntity = GetEntity(authoring.barVisualGameObject, TransformUsageFlags.NonUniformScale),
                healthTargetEntity = GetEntity(authoring.healthTargetGameObject, TransformUsageFlags.Dynamic),
            });
        }
    }
}

public struct HealthBar : IComponentData
{
    public Entity barVisualEntity;
    public Entity healthTargetEntity;
}

