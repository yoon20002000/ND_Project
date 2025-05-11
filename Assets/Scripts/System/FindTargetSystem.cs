using System.ComponentModel;
using Structs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct FindTargetSystem : ISystem
{
    private const int BUFFER_DEFAULT_CAPACITY = 5;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<FindTarget>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);
        NativeHashSet<Entity> existingTargets = new NativeHashSet<Entity>(BUFFER_DEFAULT_CAPACITY, Allocator.Temp);

        foreach ((RefRO<LocalTransform> localTransform, RefRW<FindTarget> findTarget, Entity entity)
                 in SystemAPI.Query<RefRO<LocalTransform>, RefRW<FindTarget>>().WithEntityAccess())
        {
            findTarget.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
            if (findTarget.ValueRO.Timer > 0)
            {
                continue;
            }

            findTarget.ValueRW.Timer = findTarget.ValueRO.TimerMax;

            switch (findTarget.ValueRO.eTargetingType)
            {
                case ETargetingType.Closest:
                default:
                {
                    FindClosestTargets(ref state, entity, localTransform, findTarget, ref physicsWorldSingleton,
                        ref existingTargets, ref hits);
                    break;
                }
                case ETargetingType.LowestHP:
                {
                    FindLowestHPTargets(ref state, entity, localTransform, findTarget, ref physicsWorldSingleton, ref hits);
                    break;
                }
            }
        }

        existingTargets.Dispose();
        hits.Dispose();
    }

    private bool CheckFocusTargetValid(ref SystemState state, in LocalTransform finderLocalTransform,
        in Entity targetEntity, float minDistance, float maxDistance)
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
        float dist = math.distancesq(targetLocalTransform.Position, finderLocalTransform.Position);
        return dist >= minDistance * minDistance && dist <= maxDistance * maxDistance;
    }

    private void FindClosestTargets(ref SystemState state, in Entity entity, in RefRO<LocalTransform> localTransform,
        RefRW<FindTarget> findTarget,
        ref PhysicsWorldSingleton physicsWorldSingleton,
        ref NativeHashSet<Entity> existingTargets,
        ref NativeList<DistanceHit> hits
    )
    {
        DynamicBuffer<TargetBuffer> buffer = state.EntityManager.GetBuffer<TargetBuffer>(entity);

        existingTargets.Clear();
        for (int targetBufferIndex = 0; targetBufferIndex < buffer.Length; ++targetBufferIndex)
        {
            if (CheckFocusTargetValid(ref state, localTransform.ValueRO, buffer[targetBufferIndex].targetEntity,
                    findTarget.ValueRO.MinDistance, findTarget.ValueRO.MaxDistance))
            {
                existingTargets.Add(buffer[targetBufferIndex].targetEntity);
            }
        }

        buffer.Clear();

        CollisionFilter filter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = findTarget.ValueRO.TargetLayer,
            GroupIndex = 0,
        };
        
        if (!physicsWorldSingleton.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.MaxDistance, ref hits, filter))
        {
            return;
        }

        var uniqueTargets = new NativeHashSet<Entity>(BUFFER_DEFAULT_CAPACITY, Allocator.Temp);
        var candidates = new NativeList<EntityDistancePair>(Allocator.Temp);
        for (int hitIndex = 0; hitIndex < hits.Length; ++hitIndex)
        {
            var target = hits[hitIndex].Entity;

            if (!SystemAPI.HasComponent<Unit>(target) || !SystemAPI.HasComponent<LocalTransform>(target))
            {
                continue;
            }

            Unit unit = SystemAPI.GetComponent<Unit>(target);

            if (unit.UnitType != findTarget.ValueRO.TargetingUnitType)
            {
                continue;
            }

            var targetTransform = SystemAPI.GetComponent<LocalTransform>(target);
            float distanceSq = math.distancesq(localTransform.ValueRO.Position, targetTransform.Position);

            if (distanceSq < findTarget.ValueRO.MinDistance * findTarget.ValueRO.MinDistance)
            {
                continue;
            }

            if (uniqueTargets.Add(target))
            {
                candidates.Add(new EntityDistancePair(){Entity = target, DistanceSq = distanceSq});
            }
        }

        candidates.Sort(new ClosestComparer());

        foreach (EntityDistancePair candidate in candidates)
        {
            existingTargets.Add(candidate.Entity);
        }

        foreach (Entity targetEntity in existingTargets)
        {
            if (buffer.Length >= findTarget.ValueRO.MaxTargets)
            {
                break;
            }
            buffer.Add(new TargetBuffer { targetEntity = targetEntity });    
        }
        
        uniqueTargets.Dispose();
        candidates.Dispose();
    }

    private void FindLowestHPTargets(ref SystemState state, in Entity entity, in RefRO<LocalTransform> localTransform,
        RefRW<FindTarget> findTarget,
        ref PhysicsWorldSingleton physicsWorldSingleton,
        ref NativeList<DistanceHit> hits)
    {
        DynamicBuffer<TargetBuffer> buffer = state.EntityManager.GetBuffer<TargetBuffer>(entity);
        buffer.Clear();

        hits.Clear();
        CollisionFilter filter = new CollisionFilter()
        {
            BelongsTo = ~0u,
            CollidesWith = findTarget.ValueRO.TargetLayer,
            GroupIndex = 0,
        };

        if (!physicsWorldSingleton.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.MaxDistance,
                ref hits, filter))
        {
            return;
        }

        NativeList<EntityHPPair> candidates = new NativeList<EntityHPPair>(Allocator.Temp);

        for (int i = 0; i < hits.Length; ++i)
        {
            Entity target = hits[i].Entity;

            if (!SystemAPI.HasComponent<Unit>(target) || !SystemAPI.HasComponent<Status>(target))
            {
                continue;
            }

            Unit unit = SystemAPI.GetComponent<Unit>(target);
            if (unit.UnitType != findTarget.ValueRO.TargetingUnitType)
            {
                continue;
            }

            var targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target);
            float distanceSq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);
            if (distanceSq < findTarget.ValueRO.MinDistance * findTarget.ValueRO.MinDistance)
            {
                continue;
            }

            float hp = SystemAPI.GetComponent<Status>(target).CurHP;
            candidates.Add(new EntityHPPair() { Entity = target, HP = hp });
        }

        candidates.Sort(new HealthAscendingComparer());

        int maxTargets = findTarget.ValueRO.MaxTargets;
        for (int i = 0; i < math.min(maxTargets, candidates.Length); ++i)
        {
            buffer.Add((new TargetBuffer() { targetEntity = candidates[i].Entity }));
        }
        candidates.Dispose();
    }
}