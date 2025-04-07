using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class GridSpawnerAuthoring : MonoBehaviour
{
   public GameObject gridCellPrefab;
   public int width = 10;
   public int height = 10;
   public float cellSize = 1f;
   public float gridCellFactor = 0.9f;
   private class GridBaker : Baker<GridSpawnerAuthoring>
   {
      public override void Bake(GridSpawnerAuthoring spawnerAuthoring)
      {
         Entity entity = GetEntity(spawnerAuthoring, TransformUsageFlags.None);
         AddComponent(entity, new GridSpawner
         {
            CellPrefab = GetEntity(spawnerAuthoring.gridCellPrefab, TransformUsageFlags.Renderable),
            Width = spawnerAuthoring.width,
            Height = spawnerAuthoring.height,
            CellSize = spawnerAuthoring.cellSize,
            GridCellFactor = spawnerAuthoring.gridCellFactor,
         });
      }
   }
}

public struct GridSpawner : IComponentData
{
   public Entity CellPrefab;
   public int Width;
   public int Height;
   public float CellSize;
   public float GridCellFactor;
}