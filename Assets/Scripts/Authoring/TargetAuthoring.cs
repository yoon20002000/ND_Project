using Unity.Entities;
using UnityEngine;

public class TargetAuthoring : MonoBehaviour
{
   private static int MAX_BUFFER_SIZE = 5;
   private class Baker : Baker<TargetAuthoring>
   {
      public override void Bake(TargetAuthoring authoring)
      {
         Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
         AddBuffer<TargetBuffer>(entity).EnsureCapacity(MAX_BUFFER_SIZE);
      }
   }
}
public struct TargetBuffer : IBufferElementData
{
   public Entity targetEntity;
}