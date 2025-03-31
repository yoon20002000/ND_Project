using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

partial struct FindTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);

        foreach ((RefRO<LocalTransform> localTransform, RefRW<FindTarget> findTarget, RefRW<Target> target) 
                 in SystemAPI.Query<RefRO<LocalTransform>, RefRW<FindTarget>, RefRW<Target>>())
        {
            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (findTarget.ValueRO.timer > 0.0f)
            {
                continue;
            }

            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;
            
            distanceHitList.Clear();
            CollisionFilter collisionFilter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = findTarget.ValueRO.targetLayer,
                GroupIndex = 0,
            };

            // To do : 추후 로켓 런처의 min 사거리 제한이 들어가면 로직 수정 필요 예상 
            if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.maxDistance, ref distanceHitList, collisionFilter))
            {
                foreach (DistanceHit hit in distanceHitList)
                {
                    if (SystemAPI.Exists(hit.Entity) == false || SystemAPI.HasComponent<Unit>(hit.Entity) == false)
                    {
                        continue;
                    }

                    Unit targetUnit = SystemAPI.GetComponent<Unit>(hit.Entity);
                    if (targetUnit.UnitType == findTarget.ValueRO.targettingUnitType)
                    {
                        target.ValueRW.targetEntity = hit.Entity;
                        break;
                    }
                }
            }
        }
    }
}
