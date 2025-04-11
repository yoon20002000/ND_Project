using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnerAuthoring : MonoBehaviour
{
    public float spawnPeriod = 1.0f;
    public float spawnLimitTime = 5;
    public int spawnCount = 0;
    public GameObject spawnPrefab;
    private class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new Spawner
            {
                SpawnPeriod = authoring.spawnPeriod,
                SpawnLimitTime = authoring.spawnLimitTime,
                SpawnCount = authoring.spawnCount,
                SpawnEntity = GetEntity(authoring.spawnPrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}

public struct Spawner : IComponentData
{
    public float SpawnCurTimer;
    public float SpawnPeriod;
    public float SpawnTimer;
    public float SpawnLimitTime;
    public int SpawnCurCount;
    public int SpawnCount;
    public Entity SpawnEntity;
}
