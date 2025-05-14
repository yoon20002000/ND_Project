using Unity.Entities;
using UnityEngine;

public class StatusAuthoring : MonoBehaviour
{
    [SerializeField]
    private float maxHP;
    [SerializeField]
    private float defense;
    [SerializeField]
    private float attack;
    
    private class Baker : Baker<StatusAuthoring>
    {
        public override void Bake(StatusAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new Status
            {
                CurHP = authoring.maxHP,
                MaxHp = authoring.maxHP,
                Defense = authoring.defense,
            });
        }
    }
}

public struct Status : IComponentData
{
    public float CurHP;
    public float MaxHp;
    public float Attack; // 추후 movepathalong authoring과 system에서 damage 주는거 따로 system 빼서 status attack을 이용해 처리하는게 좋을 듯 함.
    public float Defense;

    public const float MIN_HP = 1f;
    public const float MAX_HP = 999f;
    public const float MIN_DEFENSE = 0;
    public const float MAX_DEFENSE = 128f;
    public const float MIN_ATTACK = 0;
    public const float MAX_ATTACK = 128f;

    public bool OnHealthChanged;
    public bool OnMaxHealthChanged;
    public bool OnAttackChanged;
    public bool OnDefenseChanged;
}
