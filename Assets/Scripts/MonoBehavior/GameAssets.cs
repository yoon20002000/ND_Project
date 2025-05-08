using System;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public const string LAYER_NAME_UNITS = "Units";
    public const string LAYER_NAME_GRID = "Grid";
    public static GameAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public uint GetUnitLayer()
    {
        return getLayerByName(LAYER_NAME_UNITS);
    }

    public uint GetGridLayer()
    {
        return getLayerByName(LAYER_NAME_GRID);
    }

    private uint getLayerByName(string name)
    {
        return (uint) 1 << LayerMask.NameToLayer(name);
    }
}
