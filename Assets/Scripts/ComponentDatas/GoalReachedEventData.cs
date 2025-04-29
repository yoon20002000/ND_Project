using Unity.Entities;

public struct GoalReachedEventData : IComponentData, IEnableableComponent
{
    public float DamageToGoal;
}
