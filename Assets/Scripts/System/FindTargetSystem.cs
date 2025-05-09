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
        NativeList<int> removeBufferIndex = new NativeList<int>(BUFFER_DEFAULT_CAPACITY, Allocator.Temp);
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);
        foreach ((RefRO<LocalTransform> localTransform, RefRW<FindTarget> findTarget, Entity entity)
                 in SystemAPI.Query<RefRO<LocalTransform>, RefRW<FindTarget>>().WithEntityAccess())
        {
            findTarget.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
            if (findTarget.ValueRO.Timer > 0)
            {
                continue;
            }

            findTarget.ValueRW.Timer = findTarget.ValueRO.TimerMax;

            DynamicBuffer<TargetBuffer> buffer = state.EntityManager.GetBuffer<TargetBuffer>(entity);
            removeBufferIndex.Clear();

            for (int targetBufferIndex = 0; targetBufferIndex < buffer.Length; ++targetBufferIndex)
            {
                if (!CheckFocusTargetValid(ref state, localTransform.ValueRO, buffer[targetBufferIndex].targetEntity,
                        findTarget.ValueRO.MinDistance, findTarget.ValueRO.MaxDistance))
                {
                    removeBufferIndex.Add(targetBufferIndex);
                }
            }

            for (int removeIndex = removeBufferIndex.Length - 1; removeIndex >= 0; --removeIndex)
            {
                buffer.RemoveAt(removeIndex);
            }

            CollisionFilter filter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = findTarget.ValueRO.TargetLayer,
                GroupIndex = 0,
            };

            if (findTarget.ValueRO.MaxTargets <= buffer.Length)
            {
                continue;
            }

            hits.Clear();

            if (physicsWorldSingleton.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.MaxDistance,
                    ref hits, filter))
            {
                for (int hitIndex = 0; hitIndex < hits.Length; ++hitIndex)
                {
                    Entity targetEntity = hits[hitIndex].Entity;
                    if (!SystemAPI.Exists(targetEntity) || !SystemAPI.HasComponent<Unit>(targetEntity) ||
                        CheckDuplication(buffer, targetEntity))
                    {
                        continue;
                    }

                    Unit unit = SystemAPI.GetComponent<Unit>(targetEntity);
                    if (unit.UnitType != findTarget.ValueRO.TargetingUnitType)
                    {
                        continue;
                    }

                    float distance = math.distance(localTransform.ValueRO.Position,
                        SystemAPI.GetComponent<LocalTransform>(targetEntity).Position);

                    if (distance < findTarget.ValueRO.MinDistance)
                    {
                        continue;
                    }

                    buffer.Add(new TargetBuffer { targetEntity = targetEntity });

                    if (findTarget.ValueRO.eTargetSearchType == ETargetSearchType.Single ||
                        findTarget.ValueRO.MaxTargets <= buffer.Length)
                    {
                        break;
                    }
                }
            }
        }

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

    private bool CheckDuplication(in DynamicBuffer<TargetBuffer> targetEntities, in Entity targetEntity)
    {
        foreach (TargetBuffer targetBuffer in targetEntities)
        {
            if (targetBuffer.targetEntity == targetEntity)
            {
                return true;
            }
        }

        return false;
    }
}