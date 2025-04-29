using Unity.Entities;
using UnityEngine;

public class MovePathAlongAuthoring : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float damageToGoal = 1;
    private class Baker : Baker<MovePathAlongAuthoring>
    {
        public override void Bake(MovePathAlongAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new MovePathAlong
            {
                MoveSpeed = authoring.moveSpeed,
            });

            AddComponent(entity, new GoalReachedEventData
            {
                DamageToGoal = authoring.damageToGoal
            });

            AddBuffer<PathPosition>(entity);

            SetComponentEnabled<MovePathAlong>(entity, false);
        }
    }
}

public struct MovePathAlong : IComponentData, IEnableableComponent
{
    public float MoveSpeed;
    public int CurrentIndex;
}
