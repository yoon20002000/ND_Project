using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PathFindingSystem))]
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

        SystemHandle handle = state.WorldUnmanaged.GetExistingUnmanagedSystem<GridToWorldMapSystem>();
        GridToWorldMapSystem gridMapSystem = state.WorldUnmanaged.GetUnsafeSystemRef<GridToWorldMapSystem>(handle);

        foreach ((RefRW<Spawner> spawner, DynamicBuffer<PathPosition> pathBuffer, Entity entity) in SystemAPI.Query<RefRW<Spawner>, DynamicBuffer<PathPosition>>().WithEntityAccess())
        {
            if (spawner.ValueRO.SpawnCount <= spawner.ValueRO.SpawnCurCount)
            {
                continue;
            }

            spawner.ValueRW.SpawnCurTimer -= SystemAPI.Time.DeltaTime;
            spawner.ValueRW.SpawnTimer += SystemAPI.Time.DeltaTime;

            if (spawner.ValueRO.SpawnCurTimer > 0 || spawner.ValueRO.SpawnTimer >= spawner.ValueRO.SpawnLimitTime)
            {
                continue;
            }

            spawner.ValueRW.SpawnCurTimer = spawner.ValueRO.SpawnPeriod;

            Entity instantiateEntity = ecb.Instantiate(spawner.ValueRO.SpawnEntity);

            int2 spawnGrid = pathBuffer[0].Position;

            float3 spawnPosition = gridMapSystem.GetWorldPosition(spawnGrid);

            ecb.SetComponent(instantiateEntity, new LocalTransform()
            {
                Position = spawnPosition,
                Rotation = quaternion.identity,
                Scale = 1.0f,
            });

            DynamicBuffer<PathPosition> unitPathBuffer = ecb.AddBuffer<PathPosition>(instantiateEntity);
            foreach (var buffer in pathBuffer)
            {
                unitPathBuffer.Add(buffer);
            }

            ecb.SetComponentEnabled<MovePathAlong>(instantiateEntity, true);
            ecb.SetComponentEnabled<GoalReachedEventData>(instantiateEntity, false);

            ++spawner.ValueRW.SpawnCurCount;
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
