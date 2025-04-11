using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct SpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Spawner>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (RefRW<Spawner> spawner in SystemAPI.Query<RefRW<Spawner>>())
        {
            if (spawner.ValueRO.SpawnCount <= spawner.ValueRO.SpawnCurCount)
            {
                continue;
            }
            
            spawner.ValueRW.SpawnCurTimer -= SystemAPI.Time.DeltaTime;
            spawner.ValueRW.SpawnTimer += SystemAPI.Time.DeltaTime;
            
            if (spawner.ValueRO.SpawnCurTimer > 0 || spawner.ValueRO.SpawnTimer >= spawner.ValueRO.SpawnLimitTime )
            {
                continue;
            }

            spawner.ValueRW.SpawnCurTimer = spawner.ValueRO.SpawnPeriod;
            
            Entity instantiateEntity = ecb.Instantiate(spawner.ValueRO.SpawnEntity);
            
            ecb.SetComponent(instantiateEntity, new LocalTransform()
            {
                Position = new float3(1,0,1),
                Rotation = quaternion.identity,
                Scale = 1.0f,
            });
            
            // 첫 위치 지정, 생성하고 목표 지점 을 설정 해 줘야 됨
            //
            
            ++spawner.ValueRW.SpawnCurCount;
            
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
