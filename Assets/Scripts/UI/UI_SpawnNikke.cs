using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;

[DisallowMultipleComponent]
public class UI_SpawnNikke : ScrollVirtualizer
{
    public class UI_SpawnNikkeOpenData : UIParamBase
    {
        public Entity TargetEntity { get; private set; }
        public GridCell TargetCell { get; private set; }

        public UI_SpawnNikkeOpenData(Entity targetEntity, GridCell targetCell)
        {
            TargetEntity = targetEntity;
            TargetCell = targetCell;
        }
    }
    public override EUIType EuiTypeValue => EUIType.UI_SpawnNikke; 
    [SerializeField]
    private NikkeDataList nikkeDataList;
    private Entity targetEntity;
    private GridCell targetGridCell;
    protected override void Start()
    {
        base.Start();
        StartCoroutine(InitializeLater());
    }

    public override void OpenUI(UIParamBase param)
    {
        base.OpenUI(param);
        
        Assert.IsTrue(param is UI_SpawnNikkeOpenData);
        
        if (param is UI_SpawnNikkeOpenData data)
        {
            targetEntity = data.TargetEntity;
            targetGridCell = data.TargetCell;
        }
    }

    private IEnumerator InitializeLater()
    {
        yield return null;
        
        List<ScrollData> scrollDataList = new List<ScrollData>(nikkeDataList.Count);
        for (int i = 0; i < nikkeDataList.Count; ++i)
        {
            NikkeData nikkeData = nikkeDataList.GetByIndex(i);

            scrollDataList.Add(new UI_SpawnNikkeScrollData(onClickedScrollItem, nikkeData));
        }
        Initialize(scrollDataList);
    }

    private void onClickedScrollItem(NikkeData nikkeData)
    {
        Debug.LogWarning($"Warning {nikkeData.NikkeName}");
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity refEntity = entityManager.CreateEntityQuery(typeof(EntitiesReferences)).GetSingletonEntity();
        var createPrefabEntity = EntityReferenceUtil.GetNikkePrefabByName(refEntity, nikkeData.NikkeName, entityManager);
        
        Entity spawnedTower = entityManager.Instantiate(createPrefabEntity);
        entityManager.SetComponentData(spawnedTower, new LocalTransform
        {
            Position = targetGridCell.WorldPosition,
            Rotation = quaternion.identity,
            Scale = 1f
        });
        
        targetGridCell.HasTower = true;
        entityManager.SetComponentData<GridCell>(targetEntity, targetGridCell);
        CloseUI();
    }
}