using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GridCellAuthoring : MonoBehaviour
{
    private class Baker : Baker<GridCellAuthoring>
    {
        public override void Bake(GridCellAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Renderable);
            
            AddComponent(entity,new GridCell
            {
                GridPosition = int2.zero,
                WorldPosition = float3.zero,
                IsWalkable = true,
                CanBuild = false,
                HasTower = false,
            });
        }
    }
}

public struct GridCell : IComponentData
{
    public int2 GridPosition;
    public float3 WorldPosition;
    public bool IsWalkable;
    public bool CanBuild;
    public bool HasTower;
}