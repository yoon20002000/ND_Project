using TMPro;
using UnityEngine;
using UnityEngine.UI;

class UI_SpawnNikkeScrollData : ScrollData
{
    public Sprite img_Icon { get; private set; }
    public string Name { get; private set; }

    public UI_SpawnNikkeScrollData(Sprite icon, string name)
    {
        img_Icon = icon;
        Name = name;
    }
}

class UI_SpawnNikkeScrollItem : ScrollItemBase
{
    [SerializeField]
    private Image img_Icon;
    [SerializeField]
    private TextMeshProUGUI txt_Name;

    public override void SetData(ScrollData inData)
    {
        if (inData is UI_SpawnNikkeScrollData data)
        {
            img_Icon.sprite = data.img_Icon;
            txt_Name.text = data.Name;
        }
    }
}