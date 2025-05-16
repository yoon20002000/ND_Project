using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct AttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 공격 지점 추가 판정 시 이팩트 생성 필요 시 참조
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach ((RefRW<LocalTransform> localTransform, RefRW<Attack> attack, RefRO<FindTarget> findTarget, DynamicBuffer<TargetBuffer> targetBuffer, RefRO<Status> status, Entity entity)
    in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Attack>, RefRO<FindTarget>, DynamicBuffer<TargetBuffer>, RefRO<Status>>().WithEntityAccess())
        {
            if (targetBuffer.IsEmpty)
            {
                continue;
            }

            attack.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
            if (attack.ValueRO.Timer > 0)
            {
                continue;
            }

            attack.ValueRW.Timer = attack.ValueRW.MaxTimer;

            switch (attack.ValueRO.eAttackType)
            {
                case EAttackType.SingleShot:
                    default:
                {
                    foreach (TargetBuffer target in targetBuffer)
                    {
                        Entity targetEntity = target.targetEntity;
                
                        if (targetEntity == Entity.Null)
                        {
                            continue;
                        }
                
                        LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(targetEntity);
                        if (math.distancesq(localTransform.ValueRO.Position,targetLocalTransform.Position) <= findTarget.ValueRO.MaxDistance * findTarget.ValueRO.MaxDistance)
                        {
                            RefRW<Status> targetStatus = SystemAPI.GetComponentRW<Status>(targetEntity);
                            float totalDamage = Utils.DamageCalculatorUtil.CalculateDamage(targetStatus.ValueRO, status.ValueRO, attack.ValueRO);
                            float curHp = targetStatus.ValueRO.CurHP;
                    
                            targetStatus.ValueRW.CurHP = math.clamp(curHp - totalDamage,Status.MIN_HP,Status.MAX_HP);
                            targetStatus.ValueRW.OnHealthChanged = true;
                    
                            // 파티클 생성
                            Entity hitParticleEntity = ecb.Instantiate(entitiesReferences.HitParticleEntity);
                            LocalTransform particleTransform = state.EntityManager.GetComponentData<LocalTransform>(entitiesReferences.HitParticleEntity); 
                            ecb.SetComponent(hitParticleEntity, new LocalTransform
                            {
                                Position = targetLocalTransform.Position,
                                Scale = particleTransform.Scale,
                                Rotation = quaternion.identity
                            });
                        }
                    }
                    break;
                }
                case EAttackType.SplashShot:
                {
                    Entity targetEntity = Entity.Null;
                    foreach (TargetBuffer target in targetBuffer)
                    {
                        if (target.targetEntity != Entity.Null)
                        {
                            targetEntity = target.targetEntity;
                            break;
                        }
                    }

                    if (targetEntity != Entity.Null)
                    {
                        LocalTransform targetLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(targetEntity);
                        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

                        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);
                        CollisionFilter filter = new CollisionFilter
                        {
                            BelongsTo = ~0u,
                            CollidesWith = findTarget.ValueRO.TargetLayer,
                            GroupIndex = 0,
                        };
                        // 범위 내
                        if(physicsWorldSingleton.OverlapSphere(targetLocalTransform.Position, findTarget.ValueRO.MaxDistance, ref hits, filter))
                        {
                            int hitCount = 0;
                            foreach (DistanceHit hit in hits)
                            {
                                Entity splashedEntity = hit.Entity;
                                if (state.EntityManager.HasComponent<Unit>(splashedEntity) && state.EntityManager.HasComponent<Status>(splashedEntity))
                                {
                                    Unit unit = state.EntityManager.GetComponentData<Unit>(splashedEntity);
                                    if (unit.UnitType != findTarget.ValueRO.TargetingUnitType)
                                    {
                                        continue;
                                    }
                                    
                                    // 체력 감소
                                    LocalTransform splasedLocalTransform = SystemAPI.GetComponent<LocalTransform>(splashedEntity);

                                    RefRW<Status> targetStatus = SystemAPI.GetComponentRW<Status>(splashedEntity);
                                    float totalDamage = Utils.DamageCalculatorUtil.CalculateDamage(targetStatus.ValueRO,
                                        status.ValueRO, attack.ValueRO);
                                    float curHp = targetStatus.ValueRO.CurHP;

                                    targetStatus.ValueRW.CurHP =
                                        math.clamp(curHp - totalDamage, Status.MIN_HP, Status.MAX_HP);
                                    targetStatus.ValueRW.OnHealthChanged = true;
                                    ++hitCount;
                                    // 파티클 생성
                                    Entity hitParticleEntity = ecb.Instantiate(entitiesReferences.HitParticleEntity);
                                    LocalTransform particleTransform =
                                        state.EntityManager.GetComponentData<LocalTransform>(entitiesReferences.HitParticleEntity);
                                    ecb.SetComponent(hitParticleEntity, new LocalTransform
                                    {
                                        Position = splasedLocalTransform.Position,
                                        Scale = particleTransform.Scale,
                                        Rotation = quaternion.identity
                                    });
                                }
                            }
                        }
                        
                        hits.Dispose();
                    }
                    break;
                }
            }
            
            quaternion targetRotation = quaternion.identity;
            
            if (!targetBuffer.IsEmpty)
            {
                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(targetBuffer[0].targetEntity);
                float3 totalDir = targetLocalTransform.Position - localTransform.ValueRO.Position;
                if (!math.all(totalDir == float3.zero))
                {
                    targetRotation = quaternion.LookRotation(totalDir, math.up());
                }
            }

            localTransform.ValueRW.Rotation = targetRotation;
        }
    }
}
