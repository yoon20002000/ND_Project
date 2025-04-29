using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SpawnerSystem))]
partial struct MovePathAlongSystem : ISystem
{
    private NativeHashMap<int2, float3> cachedGridToWorldMap;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MovePathAlong>();
        state.RequireForUpdate<PathPosition>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        if (cachedGridToWorldMap.IsCreated == false || cachedGridToWorldMap.IsEmpty == true)
        {
            SystemHandle handle = state.WorldUnmanaged.GetExistingUnmanagedSystem<GridToWorldMapSystem>();
            GridToWorldMapSystem gridMapSystem = state.WorldUnmanaged.GetUnsafeSystemRef<GridToWorldMapSystem>(handle);
            cachedGridToWorldMap = gridMapSystem.GetMap();
        }
        
        foreach ((RefRW<MovePathAlong> movePathAlong, EnabledRefRW<MovePathAlong> enabledMovePathAlong,
                     RefRW<LocalTransform> localTransform, EnabledRefRW<GoalReachedEventData> enabledGoal, DynamicBuffer <PathPosition> pathPositions, Entity entity)
                 in SystemAPI.Query<RefRW<MovePathAlong>, EnabledRefRW<MovePathAlong>, RefRW<LocalTransform>, EnabledRefRW<GoalReachedEventData>,
                         DynamicBuffer <PathPosition>>().WithDisabled<GoalReachedEventData>().WithEntityAccess())
        {
            if (enabledMovePathAlong.ValueRO == false)
            {
                continue;
            }

            if (movePathAlong.ValueRO.CurrentIndex >= pathPositions.Length)
            {
                enabledGoal.ValueRW = true;
                enabledMovePathAlong.ValueRW = false;
                
                continue;
            }
            int currentIndex = movePathAlong.ValueRO.CurrentIndex;
            
            
            int2 pos = pathPositions[currentIndex].Position;
            float3 targetPos = new float3(pos.x, 0, pos.y);
            float3 curPos = localTransform.ValueRO.Position;

            float moveSpeed = movePathAlong.ValueRO.MoveSpeed;
            float step = moveSpeed * deltaTime;
            
            float3 dir = math.normalize(targetPos - curPos);
            float dist = math.distance(curPos,targetPos);

            if (dist <= step)
            {
                localTransform.ValueRW.Position = targetPos;
                movePathAlong.ValueRW.CurrentIndex++;
            }
            else
            {
                localTransform.ValueRW.Position += dir * step;
            }
        }
    }

    public void OnDestroy(ref SystemState state)
    {
        if (cachedGridToWorldMap.IsCreated == true)
        {
            cachedGridToWorldMap.Dispose();
        }
    }
}