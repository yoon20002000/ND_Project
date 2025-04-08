using Unity.Entities;
using UnityEngine;

public class GridSpawnerAuthoring : MonoBehaviour
{
   public GameObject gridCellPrefab;
   public float cellSize = 1f;
   public float gridCellFactor = 0.9f;
   
   private class Baker : Baker<GridSpawnerAuthoring>
   {
      public override void Bake(GridSpawnerAuthoring spawnerAuthoring)
      {
         Entity entity = GetEntity(spawnerAuthoring, TransformUsageFlags.None);
         AddComponent(entity, new GridSpawner
         {
            CellPrefab = GetEntity(spawnerAuthoring.gridCellPrefab, TransformUsageFlags.Renderable),
            CellSize = spawnerAuthoring.cellSize,
            GridCellFactor = spawnerAuthoring.gridCellFactor,
         });
      }
   }
}

public struct GridSpawner : IComponentData
{
   public Entity CellPrefab;
   public float CellSize;
   public float GridCellFactor;
}