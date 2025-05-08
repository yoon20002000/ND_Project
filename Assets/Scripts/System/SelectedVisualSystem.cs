using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SelectedVisualSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Selected>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<Selected> selected, Entity entity) in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>().WithEntityAccess())
        {
            if (!state.EntityManager.IsComponentEnabled<Selected>(entity))
            {
                RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                visualLocalTransform.ValueRW.Scale = 0;
            }
            else
            {
                RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                visualLocalTransform.ValueRW.Scale = selected.ValueRO.showScale;
            }
        }
    }
}
