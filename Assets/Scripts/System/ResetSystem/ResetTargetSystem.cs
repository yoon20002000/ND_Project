using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
partial struct ResetTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeList<int> tempNullEntities = new NativeList<int>(Allocator.Temp);
        foreach (DynamicBuffer<TargetBuffer> targetBuffers in SystemAPI.Query<DynamicBuffer<TargetBuffer>>())
        {
            tempNullEntities.Clear();
            for (int i = 0; i < targetBuffers.Length; i++)
            {
                Entity targetEntity = targetBuffers[i].targetEntity;
                if (targetEntity != Entity.Null)
                {
                    if (SystemAPI.Exists(targetEntity) == false ||
                        SystemAPI.HasComponent<LocalTransform>(targetEntity) == false)
                    {
                        tempNullEntities.Add(i);        
                    }
                }
            }

            for (int i = tempNullEntities.Length - 1; i >= 0; --i)
            {
                targetBuffers.RemoveAt(tempNullEntities[i]);
            }
        }
        tempNullEntities.Dispose();
    }
}
