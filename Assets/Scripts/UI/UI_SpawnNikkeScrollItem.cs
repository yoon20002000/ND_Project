using System;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

class UI_SpawnNikkeScrollData : ScrollData
{
    public Action<NikkeData> OnClicked { get; private set; }
    public NikkeData NikkeData { get; private set; }
    public Sprite Img_Icon => NikkeData.NikkeIcon;
    public string Name => NikkeData.NikkeDisplayName;
    
    public UI_SpawnNikkeScrollData(Action<NikkeData> onClicked, NikkeData nikkeData)
    {
        OnClicked = onClicked;
        NikkeData = nikkeData;
    }
}

class UI_SpawnNikkeScrollItem : ScrollItemBase
{
    [SerializeField]
    private Image img_Icon;
    [SerializeField]
    private TextMeshProUGUI txt_Name;

    [SerializeField] 
    private Button btn_Click;

    private Entity targetEntity;
    private GridCell targetGridCell;
    private Action<NikkeData> onClickedEvent;
    private NikkeData nikkeData;

    private void Start()
    {
        btn_Click.onClick.AddListener(onClicked);
    }

    public override void SetData(ScrollData inData)
    {
        targetGridCell = default;
        if (inData is UI_SpawnNikkeScrollData data)
        {
            onClickedEvent = data.OnClicked;
            nikkeData = data.NikkeData;
            img_Icon.sprite = data.Img_Icon;
            txt_Name.text = data.Name;
        }
    }

    private void onClicked()
    {
        if (onClickedEvent != null)
        {
            onClickedEvent.Invoke(nikkeData);
        }
    }
}