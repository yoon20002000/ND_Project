using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridSelectManager : MonoBehaviour
{
    [SerializeField] 
    private Camera mainCamera;
    [SerializeField]
    private GameObject towerPrefab;
    
    private EntityManager entityManager;
    private Entity towerPrefabEntity;
    
    private const float RAY_DISTANCE_START = 0f;
    private const float RAY_DISTANCE_END = 9999f;
    
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // UI 시 무시
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            // Reset
            EntityQuery entityQuery =  new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
            for (int selectedIndex = 0; selectedIndex < selectedArray.Length; selectedIndex++)
            {
                entityManager.SetComponentEnabled<Selected>(entityArray[selectedIndex], false);
                Selected selected = selectedArray[selectedIndex];
                selected.onDeselected = true;
                selectedArray[selectedIndex] = selected;
                entityManager.SetComponentData(entityArray[selectedIndex], selected);
            }

            // Ray 사전 준비
            entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            UnityEngine.Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastInput raycastInput = new RaycastInput
            {
                Start = cameraRay.GetPoint(RAY_DISTANCE_START),
                End = cameraRay.GetPoint(RAY_DISTANCE_END),
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = GameAssets.Instance.GetGridLayer(),
                    GroupIndex = 0
                }
            };

            // Ray Hit 발생 시
            if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
            {
                // Grid Cell 이고 Selection 있을 시
                if (entityManager.HasComponent<GridCell>(raycastHit.Entity) && entityManager.HasComponent<Selected>(raycastHit.Entity))
                {
                    GridCell gridCell = entityManager.GetComponentData<GridCell>(raycastHit.Entity);
                    if (gridCell is { CanBuild: true, HasTower: false })
                    {
                        Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
                        selected.onSelected = true;
                        entityManager.SetComponentData(raycastHit.Entity, selected);
                        entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);    
                        
                        // Entity refEntity = entityManager.CreateEntityQuery(typeof(EntitiesReferences)).GetSingletonEntity();
                        //
                        // Entity createPrefabEntity =
                        //     EntityReferenceUtil.GetNikkePrefabByName(refEntity, "Scarlet", entityManager);
                        //
                        // Entity spawnedTower = entityManager.Instantiate(createPrefabEntity);
                        // entityManager.SetComponentData(spawnedTower, new LocalTransform
                        // {
                        //     Position = gridCell.WorldPosition,
                        //     Rotation = quaternion.identity,
                        //     Scale = 1f
                        // });
                        //
                        // gridCell.HasTower = true;
                        // entityManager.SetComponentData<GridCell>(raycastHit.Entity, gridCell);

                        UIManager.Instance.OpenUI(UIEnum.UI_SpawnNikke, mainCamera, gridCell.WorldPosition);
                    }
                }
            }
        }
    }
}