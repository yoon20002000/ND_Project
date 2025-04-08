using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
partial struct GridSpawnerSystem : ISystem
{
    public enum EGridType
    {
        Moveable = 0,
        Blocked = 1,
        Placeable = 2,
    }
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridSpawner>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        GridSpawner gridSpawner = SystemAPI.GetSingleton<GridSpawner>();
        Entity gridCellEntity = gridSpawner.CellPrefab;
        foreach ((RefRO<StageLoader> stageLoader, DynamicBuffer<StageMapBuffer> stageMapBuffers, Entity entity) in SystemAPI.Query<RefRO<StageLoader>,DynamicBuffer<StageMapBuffer>>().WithEntityAccess())
        {
            int width = stageLoader.ValueRO.Width;
            int height = stageLoader.ValueRO.Height;
            NativeArray<StageMapBuffer> map = stageMapBuffers.AsNativeArray();
            
            for (int x = 0; x < width; ++x)
            {
                for (int z = 0; z < height; ++z)
                {
                    int flippedZ = (height - 1) - z; 
                    int index = flippedZ * width + x;
                    int cellType = map[index].StageType;
                    
                    Entity instanceEntity = ecb.Instantiate(gridCellEntity);
                    
                    float3 worldPos = new float3(x * gridSpawner.CellSize, 0, z * gridSpawner.CellSize);
                    ecb.SetComponent(instanceEntity, new LocalTransform()
                    {
                        Position = worldPos,
                        Rotation = quaternion.identity,
                        Scale = gridSpawner.CellSize * gridSpawner.GridCellFactor
                    });
        
                    ecb.SetComponent(instanceEntity, new GridCell()
                    {
                        GridPosition = new int2(x, z),
                        WorldPosition = worldPos,
                        IsWalkable = cellType == (int)EGridType.Moveable,
                        CanBuild = cellType == (int)EGridType.Placeable,
                        HasTower = false,
                    });
                }
            }
            ecb.DestroyEntity(entity);
        }
        Entity gridSpawnerEntity = SystemAPI.GetSingletonEntity<GridSpawner>();
        ecb.DestroyEntity(gridSpawnerEntity);
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        state.Enabled = false;
    }
}
