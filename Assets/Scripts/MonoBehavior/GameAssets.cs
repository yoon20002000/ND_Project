using System;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public const string LAYER_NAME_UNITS = "Units";
    public static GameAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public uint GetUnitLayer()
    {
        return getLayerByName(LAYER_NAME_UNITS);
    }

    private uint getLayerByName(string name)
    {
        return (uint) 1 << LayerMask.NameToLayer(name);
    }
}
