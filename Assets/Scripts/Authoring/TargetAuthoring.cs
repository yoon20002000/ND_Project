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
// IBufferElementData를 이용해서 여러 Target 저장 할 수 있어야 됨

public struct Target : IComponentData
{
   public Entity targetEntity;
}