using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UI_SpawnNikke : ScrollVirtualizer
{
    [SerializeField] private NikkeDataList nikkeDataList;

    private void Start()
    {
        StartCoroutine(InitializeLater());
    }

    private IEnumerator InitializeLater()
    {
        yield return null;
        
        List<ScrollData> scrollDataList = new List<ScrollData>(nikkeDataList.Count);
        for (int i = 0; i < nikkeDataList.Count; ++i)
        {
            NikkeData nikkeData = nikkeDataList.GetByIndex(i);
            scrollDataList.Add(new UI_SpawnNikkeScrollData(nikkeData.NikkeIcon, nikkeData.NikkeName));
        }
        Initialize(scrollDataList);
    }
}