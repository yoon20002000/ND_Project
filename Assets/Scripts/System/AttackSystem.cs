using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
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
            
            float3 totalDir;
            
            foreach (TargetBuffer target in targetBuffer)
            {
                if (target.targetEntity == Entity.Null)
                {
                    continue;
                }
                
                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.targetEntity);
                if (math.distancesq(localTransform.ValueRO.Position,targetLocalTransform.Position) <= findTarget.ValueRO.MaxDistance * findTarget.ValueRO.MaxDistance)
                {
                    RefRW<Status> targetStatus = SystemAPI.GetComponentRW<Status>(target.targetEntity);
                    float totalDamage = Utils.DamageCalculatorUtil.CalculateDamage(targetStatus.ValueRO, status.ValueRO, attack.ValueRO);
                    float curHp = targetStatus.ValueRO.CurHP;
                    
                    targetStatus.ValueRW.CurHP = math.clamp(curHp - totalDamage,Status.MIN_HP,Status.MAX_HP);
                    targetStatus.ValueRW.OnHealthChanged = true;
                }
            }
            
            quaternion targetRotation = quaternion.identity;
            
            if (!targetBuffer.IsEmpty)
            {
                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(targetBuffer[0].targetEntity);
                totalDir = targetLocalTransform.Position - localTransform.ValueRO.Position;
                targetRotation = quaternion.LookRotation(totalDir, math.up());
            }

            localTransform.ValueRW.Rotation = targetRotation;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
