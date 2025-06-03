using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public enum EUILayer
{
    Main = 0,
    Popup,
}

[CreateAssetMenu(fileName = "UILayer", menuName = "Scriptable Objects/UILayer")]
public class UILayer : ScriptableObject
{
    [SerializeField]
    private EUILayer eUILayer = EUILayer.Main;
    public EUILayer UILayerType => eUILayer;
    [SerializeField]
    private GameObject layerPrefab = null;
    public GameObject LayerPrefab => layerPrefab;
    [SerializeField] 
    private int sortingOrder = 0;
    public int SortingOrder => sortingOrder;
}

[CreateAssetMenu(fileName = "UILayerGroup", menuName = "Scriptable Objects/UILayerGroup")]
public class UILayerGroup : ScriptableObject
{
    [SerializeField]
    private List<UILayer> uiLayers = new List<UILayer>();

    public UILayer this[int index] => uiLayers[index];

    public void InitLayerGroup()
    {
        uiLayers.Sort((a,b)=> a.SortingOrder < b.SortingOrder ? -1 : 1);
    }
    public int Count => uiLayers.Count;
}