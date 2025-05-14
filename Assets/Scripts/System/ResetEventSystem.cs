using Unity.Burst;
using Unity.Entities;
[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
partial struct ResetEventSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        #region Reset Selected
        foreach (RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
        {
            selected.ValueRW.onSelected = false;
            selected.ValueRW.onDeselected = false;
        }
        #endregion

        #region Reset Status
        foreach (RefRW<Status> status in SystemAPI.Query<RefRW<Status>>())
        {
            status.ValueRW.OnHealthChanged = false;
            status.ValueRW.OnMaxHealthChanged = false;
            status.ValueRW.OnAttackChanged = false;
            status.ValueRW.OnAttackChanged = false;
            status.ValueRW.OnDefenseChanged = false;
        }
        #endregion
    }
}
