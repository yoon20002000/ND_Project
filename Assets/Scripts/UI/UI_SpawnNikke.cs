using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Utils;

[DisallowMultipleComponent]
public class UI_SpawnNikke : ScrollVirtualizer
{
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

    public override void OpenUI()
    {
        base.OpenUI();
        
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
        // EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // Entity refEntity = entityManager.CreateEntityQuery(typeof(EntitiesReferences)).GetSingletonEntity();
        // var entity = EntityReferenceUtil.GetNikkePrefabByName(refEntity, nikkeData.NikkeName, entityManager);
        // 생성 코드 추가 필요
    }
}