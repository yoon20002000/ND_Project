using Unity.Burst;
using Unity.Entities;

partial struct DeathSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<Status>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // EndSimulationEntityCommandBufferSystem의 ecb를 가져와서 쓰기 때문에
        // 프레임이 끝날 때 자동으로 ecb.Playback 호출
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach ((RefRO<Status> status, Entity entity)
                    in SystemAPI.Query<RefRO<Status>>().WithEntityAccess())
        {
            if (status.ValueRO.CurHP > Status.MIN_HP)
            {
                continue;
            }
            ecb.DestroyEntity(entity);
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
