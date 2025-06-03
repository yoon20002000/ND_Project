using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "UIList", menuName = "Scriptable Objects/UIList")]
public class UIList : ScriptableObject
{
    [Serializable]
    public struct UIPrefab
    {
        public UIEnum uiEnum;
        public GameObject Prefab;
    }
    [SerializeField]
    private List<UIPrefab> uiPrefabs = new List<UIPrefab>();

    public GameObject GetUIPrefab(UIEnum uiEnum)
    {
        for (int uiIndex = 0; uiIndex < uiPrefabs.Count; ++uiIndex)
        {
            if (uiPrefabs[uiIndex].uiEnum == uiEnum)
            {
             return uiPrefabs[uiIndex].Prefab;   
            }
        }
        return null;
    }
}