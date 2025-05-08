using Unity.Entities;
using UnityEngine;

class SelectedAuthoring : MonoBehaviour
{
    public GameObject VisualEntity;
    public float ShowScale;
    class Baker : Baker<SelectedAuthoring>
    {
        public override void Bake(SelectedAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity,new Selected()
            {
                visualEntity = GetEntity(authoring.VisualEntity, TransformUsageFlags.Dynamic),
                showScale = authoring.ShowScale,
            });
        }
    }
}

public struct Selected : IComponentData, IEnableableComponent
{
    public Entity visualEntity;
    public float showScale;

    public bool onSelected;
    public bool onDeselected;
}