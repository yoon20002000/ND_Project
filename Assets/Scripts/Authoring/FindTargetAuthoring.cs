using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public enum ETargetSearchType
{
    Single,
    Multiple,
}
public enum ETargetingType
{
    Closest,       // 가장 가까운 적
    LowestHP,      // 가장 HP 낮은 적
}

public class FindTargetAuthoring : MonoBehaviour
{
    [SerializeField]
    private EUnitType targetingUnitType;
    [SerializeField]
    private float minDistance;
    [SerializeField]
    private float maxDistance;
    [SerializeField] 
    private int maxTargets; 
    [SerializeField]
    private float timerMax;
    [SerializeField]
    private LayerMask targetLayer;
    private ETargetSearchType eSearchType;
    [SerializeField] 
    private ETargetingType eTargetingType;
    
    private class Baker : Baker<FindTargetAuthoring>
    {
        public override void Bake(FindTargetAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new FindTarget
            {
                TargetingUnitType = authoring.targetingUnitType,
                MinDistance =  authoring.minDistance,
                MaxDistance = authoring.maxDistance,
                MaxTargets = authoring.maxTargets,
                TimerMax = authoring.timerMax,
                TargetLayer = (uint)authoring.targetLayer.value,
                eTargetSearchType = authoring.maxTargets > 1 ? ETargetSearchType.Multiple : ETargetSearchType.Single,
                eTargetingType = authoring.eTargetingType,
            });
        }
    }
}

public struct FindTarget : IComponentData
{
    public float MinDistance;
    public float MaxDistance;
    public float MaxTargets;
    public EUnitType TargetingUnitType;
    public float Timer;
    public float TimerMax;
    public uint TargetLayer;
    public ETargetSearchType eTargetSearchType;
    public ETargetingType eTargetingType;
}
