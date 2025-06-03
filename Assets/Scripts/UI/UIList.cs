using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIList", menuName = "Scriptable Objects/UIList")]
public class UIList : ScriptableObject
{
    [Serializable]
    public struct UIPrefab
    {
        public EUIType eUIType;
        public GameObject Prefab;
    }
    [SerializeField]
    private List<UIPrefab> uiPrefabs = new List<UIPrefab>();

    public GameObject GetUIPrefab(EUIType euiType)
    {
        for (int uiIndex = 0; uiIndex < uiPrefabs.Count; ++uiIndex)
        {
            if (uiPrefabs[uiIndex].eUIType == euiType)
            {
             return uiPrefabs[uiIndex].Prefab;   
            }
        }
        return null;
    }
}