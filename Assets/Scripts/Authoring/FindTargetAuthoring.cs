using System;
using Unity.Entities;
using UnityEngine;

public enum ETargetSearchType
{
    Single,
    Multiple,
}

public class FindTargetAuthoring : MonoBehaviour
{
    public EUnitType targetingUnitType;
    public float minDistance;
    public float maxDistance;
    public float timerMax;
    public LayerMask targetLayer;
    
    [SerializeField]
    private ETargetSearchType searchType;
    
    private class Baker : Baker<FindTargetAuthoring>
    {
        public override void Bake(FindTargetAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new FindTarget
            {
                targettingUnitType = authoring.targetingUnitType,
                minDistance =  authoring.minDistance,
                maxDistance = authoring.maxDistance,
                timerMax = authoring.timerMax,
                targetLayer = (uint)authoring.targetLayer.value,
                searchType = authoring.searchType,
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
    public ETargetSearchType searchType;
}
