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
    private float splashRadius;
    [SerializeField]
    private float damageAmount;
    [SerializeField]
    private float defensePierce;
    [SerializeField]
    private float maxTimer;
    class Baker : Baker<AttackAuthoring>
    {
        public override void Bake(AttackAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new Attack
            {
                eAttackType = authoring.splashRadius > 0 ? EAttackType.SplashShot : EAttackType.SingleShot,
                DamageAmount = authoring.damageAmount,
                DefensePierce = authoring.defensePierce,
                Timer = authoring.maxTimer,
                MaxTimer = authoring.maxTimer,
            });
        }
    }    
}

public struct Attack : IComponentData
{
    public EAttackType eAttackType;
    public float DamageAmount;
    public float DefensePierce;
    public float SplashRadius;

    public float Timer;
    public float MaxTimer;
    
    public const float MIN_DAMAGE = 0f;  
    public const float MAX_DAMAGE = 9999f; 
}