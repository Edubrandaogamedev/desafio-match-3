using System;
using UnityEngine;

[Flags] 
public enum TileEffect
{
    Default = 0,
    Special_Clear_Row = 1 << 0,
    Special_Clear_Column = 1 << 1,
    Special_Clear_Area = 1 << 2,
    Special_Clear_Color = 1 << 3
}

[Serializable]
public struct TileData
{
    [SerializeField]
    private string key;
    [SerializeField]
    private TileEffect effects;
    [SerializeField] 
    private Sprite sprite;

    public string Key => key;
    public Sprite Sprite => sprite;
    public TileEffect Effects => effects;
}
