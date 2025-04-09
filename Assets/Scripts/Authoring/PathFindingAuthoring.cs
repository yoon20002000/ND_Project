using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PathFindingAuthoring : MonoBehaviour
{
  public Vector2Int start;
  public Vector2Int end;
  private class Baker : Baker<PathFindingAuthoring>
  {
    public override void Bake(PathFindingAuthoring authoring)
    {
      Entity entity = GetEntity(authoring, TransformUsageFlags.None);
      AddComponent(entity, new PathFinding
      {
        Start = new int2(authoring.start.x,authoring.start.y),
        End = new int2(authoring.end.x, authoring.end.y),
      });
    }
  }
}

public struct PathFinding : IComponentData
{
  public int2 Start;
  public int2 End;
}

public struct PathPosition : IBufferElementData
{
  public int2 Position;
}