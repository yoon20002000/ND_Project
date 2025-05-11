using Unity.Entities;
using UnityEngine;

class TowerAuthoring : MonoBehaviour
{
    public float AttackRange = 4;
    public float AttackDamage = 5;
    class Baker : Baker<TowerAuthoring>
    {
        public override void Bake(TowerAuthoring authoring)
        {
        
        }
    }
}

struct Tower : IComponentData
{
    float attackRange;
    float attackDamage;
}