using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(GridSpawnerSystem))] 
partial struct GridCellInitSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridCell>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        EntityManager entityManager = state.EntityManager;

        foreach ((RefRO<GridCell> gridCell, Entity entity) in SystemAPI.Query<RefRO<GridCell>>().WithEntityAccess())
        {
            bool isMovable = gridCell.ValueRO.IsWalkable;
            bool isPlaceable = gridCell.ValueRO.CanBuild;
            if (entityManager.HasComponent<URPMaterialPropertyBaseColor>(entity))
            {
                ecb.SetComponent(entity, new URPMaterialPropertyBaseColor(){Value = GetGridColor(isMovable, isPlaceable)});
            }
            
            if (entityManager.HasComponent<LinkedEntityGroup>(entity))
            {
                var linkedGroup = entityManager.GetBuffer<LinkedEntityGroup>(entity);
                foreach (var linked in linkedGroup)
                {
                    if (entityManager.HasComponent<URPMaterialPropertyBaseColor>(linked.Value))
                    {
                        ecb.SetComponent(linked.Value, new URPMaterialPropertyBaseColor
                        {
                            Value = GetGridColor(isMovable, isPlaceable)
                        });
                    }
                }
            }
        }
        ecb.Playback(entityManager);
        ecb.Dispose();
        
        state.Enabled = false;
    }
    private float4 GetGridColor(bool isMovable, bool isPlaceable)
    {
        if (isMovable)
        {
            return new float4(0f, 1f, 0f, 1f); // Green
        }

        if (isPlaceable)
        {
            return new float4(0f, 0f, 1f, 1f); // Blue
        }

        return new float4(1f, 0f, 0f, 1f); // Red
    }
}
