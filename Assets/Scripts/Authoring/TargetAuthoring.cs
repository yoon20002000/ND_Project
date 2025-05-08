using Unity.Entities;
using UnityEngine;

public class TargetAuthoring : MonoBehaviour
{
   private class Baker : Baker<TargetAuthoring>
   {
      public override void Bake(TargetAuthoring authoring)
      {
         Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
         AddComponent(entity, new Target
         {
            
         });
      }
   }
}

public struct Target : IComponentData
{
   public Entity targetEntity; // Array로 변경 필요
}