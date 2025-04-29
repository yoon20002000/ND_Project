using Unity.Burst;
using Unity.Entities;
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(MovePathAlongSystem))]
partial struct GoalReachedSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GoalReachedEventData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach ((RefRO<GoalReachedEventData> eventData, Entity entity) in SystemAPI.Query<RefRO<GoalReachedEventData>>().WithEntityAccess().WithAll<GoalReachedEventData>())
        {
            float damage = eventData.ValueRO.DamageToGoal;
            // 위 damage만 큼 체력 까기

            // 삭제
            ecb.SetComponentEnabled<GoalReachedEventData>(entity, false);

            ecb.DestroyEntity(entity);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
