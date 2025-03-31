using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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
            
            Entity targetEntity = Entity.Null;
            
            // 타겟이 존재하고 해당 타겟이 내 범위 안에 있을 경우 해당 타겟을 우선시 한다.
            if (checkFocusTargetValid(ref state, localTransform.ValueRO, target.ValueRO.targetEntity, findTarget.ValueRO.maxDistance))
            {
                continue;
            }
            // 만약 타겟이 존재하지 않고, 기존 타겟이 범위 밖이라면 새로운 적을 타겟팅 한다.
            else 
            {
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
                            targetEntity = hit.Entity;
                            UnityEngine.Debug.Log("new target setted");
                            break;
                        }
                    }
                }

                target.ValueRW.targetEntity = targetEntity;
            }
        }
    }

    bool checkFocusTargetValid(ref SystemState state,LocalTransform finderLocalTransform, Entity targetEntity, float maxDistance)
    {
        if (targetEntity == Entity.Null || SystemAPI.Exists(targetEntity) == false)
        {
            return false;
        }

        if (SystemAPI.HasComponent<LocalTransform>(targetEntity) == false)
        {
            return false;
        }

        LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(targetEntity);
        
        return math.distancesq(targetLocalTransform.Position, finderLocalTransform.Position) <= maxDistance * maxDistance;
    }
}
