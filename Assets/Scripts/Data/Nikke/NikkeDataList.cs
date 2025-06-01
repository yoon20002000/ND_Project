using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NikkeDataList", menuName = "Scriptable Objects/NikkeDataList")]
public class NikkeDataList : ScriptableObject
{
    public List<NikkeData> NikkeData;

    private Dictionary<string, NikkeData> nikkeByName;

    private bool bIsInitialized = false;

    public void Init()
    {
        if (bIsInitialized)
        {
            return;
        }

        nikkeByName = new Dictionary<string, NikkeData>(NikkeData.Capacity);

        foreach (var data in NikkeData)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.NikkeName))
            {
                Debug.LogWarning($"Name 설정이 비어있습니다.");
                continue;
            }

            if (!nikkeByName.ContainsKey(data.NikkeName))
            {
                Debug.LogWarning($"중복된 Nikke 이름 발견 : {data.NikkeName}");
                continue;
            }

            nikkeByName.Add(data.NikkeName, data);
        }

        bIsInitialized = true;
    }

    public NikkeData GetByName(string name)
    {
        Init();
        return nikkeByName.GetValueOrDefault(name);
    }

    public NikkeData GetByIndex(int index)
    {
        return index >= 0 && index < NikkeData.Count ? NikkeData[index] : null;
    }

    public int Count => NikkeData != null ? NikkeData.Count : 0;
}
