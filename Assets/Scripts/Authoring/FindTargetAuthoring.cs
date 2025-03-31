using System;
using Unity.Entities;
using UnityEngine;

public class FindTargetAuthoring : MonoBehaviour
{
    public EUnitType targettingUnitType;
    public float minDistance;
    public float maxDistance;
    public float timerMax;
    public LayerMask targetLayer;
    private class Baker : Baker<FindTargetAuthoring>
    {
        public override void Bake(FindTargetAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new FindTarget
            {
                targettingUnitType = authoring.targettingUnitType,
                minDistance =  authoring.minDistance,
                maxDistance = authoring.maxDistance,
                timerMax = authoring.timerMax,
                targetLayer = (uint)authoring.targetLayer.value,
            });
        }
    }
}

public struct FindTarget : IComponentData
{
    public float minDistance;
    public float maxDistance;
    public EUnitType targettingUnitType;
    public float timer;
    public float timerMax;
    public uint targetLayer;
}
