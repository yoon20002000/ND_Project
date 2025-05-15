using Unity.Entities;
using UnityEngine;

class HitParticleAuthoring : MonoBehaviour
{
    [SerializeField]
    private float timer;
    class Baker : Baker<HitParticleAuthoring>
    {
        public override void Bake(HitParticleAuthoring authoring)
        {
            Entity entity = GetEntity(authoring,TransformUsageFlags.None);
            AddComponent(entity, new HitParticle
            {
                Timer = authoring.timer,
            });
        }
    }
}

public struct HitParticle : IComponentData
{
    public float Timer;
}


