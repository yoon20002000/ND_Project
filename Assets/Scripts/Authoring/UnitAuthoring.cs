using Unity.Entities;
using UnityEngine;

public enum EUnitType
{
    Nikke,
    Rapture,
}

public class UnitAuthoring : MonoBehaviour
{
    public EUnitType UnitType;
    private class Baker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            Entity entity = GetEntity(authoring,TransformUsageFlags.Dynamic);
            AddComponent(entity, new Unit
            {
                UnitType = authoring.UnitType,
            });
        }
    }
}

public struct Unit : IComponentData
{
    public EUnitType UnitType;
}