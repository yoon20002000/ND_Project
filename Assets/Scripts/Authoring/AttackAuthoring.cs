using Unity.Entities;
using UnityEngine;

public enum EAttackType
{
    SingleShot,
    SplashShot
}

class AttackAuthoring : MonoBehaviour
{
    [SerializeField]
    private EAttackType eAttackType;
    [SerializeField]
    private float damageAmount;
    [SerializeField]
    private float defensePierce;
    class Baker : Baker<AttackAuthoring>
    {
        public override void Bake(AttackAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new Attack
            {
                eAttackType = authoring.eAttackType,
                DamageAmount = authoring.damageAmount,
                DefensePierce = authoring.defensePierce,
            });
        }
    }    
}

public struct Attack : IComponentData
{
    public EAttackType eAttackType;
    public float DamageAmount;
    public float DefensePierce;
}