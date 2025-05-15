using Unity.Burst;
using Unity.Entities;

partial struct ParticleDestroySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HitParticle>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((RefRW<HitParticle> hitParticle, Entity entity) in SystemAPI.Query<RefRW<HitParticle>>().WithEntityAccess())
        {
            hitParticle.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
            if (hitParticle.ValueRO.Timer <= 0)
            {
                ecb.DestroyEntity(entity);
            }
        }
    }
}
