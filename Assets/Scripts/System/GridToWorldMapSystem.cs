using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(GridSpawnerSystem))]
public partial struct GridToWorldMapSystem : ISystem
{
    private NativeHashMap<int2, float3> GridToWorldMap;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridCell>(); // GridCell이 있을 때만 실행
        GridToWorldMap = new NativeHashMap<int2, float3>(1000, Allocator.Persistent);
    }

    public void OnDestroy(ref SystemState state)
    {
        if (GridToWorldMap.IsCreated)
        {
            GridToWorldMap.Dispose();
        }
    }

    public void OnUpdate(ref SystemState state)
    {
        if (GridToWorldMap.Count > 0)
        {
            return; // 이미 생성됨
        }
        
        foreach (var (cell, _) in SystemAPI.Query<GridCell>().WithEntityAccess())
        {
            GridToWorldMap.TryAdd(cell.GridPosition, cell.WorldPosition);
        }
    }
    public bool TryGetWorldPos(int2 gridPos, out float3 worldPos)
    {
        return GridToWorldMap.TryGetValue(gridPos, out worldPos);
    }

    public NativeHashMap<int2, float3> GetMap() => GridToWorldMap;
    public float3 GetWorldPosition(int2 xy)
    {
        float3 worldPos = float3.zero;

        TryGetWorldPos(xy, out worldPos);

        return worldPos;
    }
}