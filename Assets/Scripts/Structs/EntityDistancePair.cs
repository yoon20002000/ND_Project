using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct EntityDistancePair
{
    public Entity Entity;
    public float DistanceSq;
}

[BurstCompile]
public struct ClosestComparer : IComparer<EntityDistancePair>
{
    float3 originPos;
    ComponentLookup<LocalTransform> LocalTransformLookup;
    
    public int Compare(EntityDistancePair x, EntityDistancePair y)
    {
        return x.DistanceSq.CompareTo(y.DistanceSq);
    }
}