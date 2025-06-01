using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ScrollVirtualizer : MonoBehaviour
{
    [SerializeField]
    private ScrollRect scrollRect;
    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private ScrollItemBase scrollItemPrefab;
    [SerializeField]
    private float itemSize = 100.0f;
    [SerializeField]
    private int extraBuffer = 2;

    private List<ScrollData> dataList;
    private List<ScrollItemBase> itemPool;

    private bool bIsVertical => scrollRect.vertical;
    private int poolSize;
    private Vector2 lastAnchoredPos;

    public void Initialize(List<ScrollData> data)
    {
        Assert.IsNotNull(content, "content is null");
        
        dataList = data;
        if (!scrollRect.vertical && !scrollRect.horizontal)
        {
            Debug.LogError("ScrollRect must be vertical or horizontal");
            return;
        }
        
        var viewportRect = scrollRect.viewport.rect;
        float viewportSize = bIsVertical ? viewportRect.height : viewportRect.width;

        poolSize = Mathf.Min(dataList.Count, Mathf.CeilToInt(viewportSize / itemSize) + extraBuffer); 

        itemPool = new List<ScrollItemBase>(poolSize);
        Vector2 contentSize = content.sizeDelta;
        if (bIsVertical)
        {
            contentSize.y = dataList.Count * itemSize;
        }
        else
        {
            contentSize.x = dataList.Count * itemSize;
        }
        content.sizeDelta = contentSize;

        for (int i = 0; i < poolSize; ++i)
        {
            var item = Instantiate(scrollItemPrefab, content);
            itemPool.Add(item);
        }

        UpdateVisibleItems(true);
    }

    private void Update()
    {
        if ((Vector2.Distance(lastAnchoredPos, content.anchoredPosition) > itemSize * 0.1f))
        {
            UpdateVisibleItems();
        }
    }

    private void UpdateVisibleItems(bool bForceUpdate = false)
    {
        lastAnchoredPos = content.anchoredPosition;
        
        float offset = bIsVertical ? lastAnchoredPos.y : lastAnchoredPos.x;
        int firstIndex = Mathf.FloorToInt(offset / itemSize);

        for (int i = 0; i < itemPool.Count; ++i)
        {
            int dataIndex = firstIndex + i;
            if (dataIndex < 0 || dataIndex >= itemPool.Count)
            {
                continue;
            }

            ScrollItemBase item = itemPool[i];
            
            // to do layout도 받아서 처리 할 수 있게 해야 됨
            //float baseOffset = layout.paddingStart + dataIndex * (itemSize + layout.spacing);
            float baseOffset = 10 + dataIndex * (itemSize + 10);
            
            Vector2 anchoredPos = bIsVertical
                ? new Vector2(0, -baseOffset)
                : new Vector2(baseOffset, -20); // to do 주 offset 값 외 값 처리 로직 추가 필요
            item.GetRectTransform().anchoredPosition = anchoredPos;
            item.SetData(dataList[dataIndex]);
        }
    }
}
