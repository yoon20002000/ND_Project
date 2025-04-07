using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
partial struct GridSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridSpawner>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach ((RefRO<GridSpawner> gridSpawner, Entity entity) in SystemAPI.Query<RefRO<GridSpawner>>().WithEntityAccess())
        {
            GridSpawner spawner = gridSpawner.ValueRO;
            for (int x = 0; x < spawner.Width; ++x)
            {
                for (int z = 0; z < spawner.Height; ++z)
                {
                    Entity instanceEntity = ecb.Instantiate(spawner.CellPrefab);
                    float3 worldPos = new float3(x * spawner.CellSize, 0, z * spawner.CellSize);
                    ecb.SetComponent(instanceEntity, new LocalTransform()
                    {
                        Position = worldPos,
                        Rotation = quaternion.identity,
                        Scale = spawner.CellSize * spawner.GridCellFactor
                    });
                    
                    ecb.SetComponent(instanceEntity, new GridCell()
                    {
                        GridPosition = new int2(x, z),
                        WorldPosition = worldPos,
                        IsWalkable = true,
                        CanBuild = false,
                        HasTower = false,
                    });
                }
            }
            ecb.DestroyEntity(entity);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
