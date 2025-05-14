using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthBarSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HealthBar>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        Vector3 cameraForward = Vector3.zero;

        if (Camera.main != null)
        {
            cameraForward = Camera.main.transform.forward;
        }
        
        foreach ((RefRW<LocalTransform> localTransform, RefRO<HealthBar> healthBar) in SystemAPI
                     .Query<RefRW<LocalTransform>, RefRO<HealthBar>>())
        {
            LocalTransform parentLocalTransform =
                SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.healthTargetEntity);

            // 실질적으로 화면에 표시 시 카메라 방향을 바라보도록
            if (localTransform.ValueRO.Scale == 1f)
            {
                localTransform.ValueRW.Rotation =
                    parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
            }

            Status status = SystemAPI.GetComponent<Status>(healthBar.ValueRO.healthTargetEntity);

            if (status.OnHealthChanged == false)
            {
                continue;
            }

            float healthNormalized = status.CurHP / status.MaxHp;

            if (healthNormalized == 1f)
            {
                localTransform.ValueRW.Scale = 0;
            }
            else
            {
                localTransform.ValueRW.Scale = 1;
            }

            // PostTransformMatrix : 월드 변환 행렬
            RefRW<PostTransformMatrix> barVisualPostTransformMatrix =
                SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);
            barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);
        }
    }
}