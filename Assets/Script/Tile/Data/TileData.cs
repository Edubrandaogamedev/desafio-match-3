using System;
using UnityEngine;

[Serializable]
public struct TileData
{
    [SerializeField]
    private string key;
    [SerializeField] 
    private TileType type;
    [SerializeField]
    private TileEffect effects;
    [SerializeField] 
    private Sprite sprite;

    public string Key => key;
    public TileType Type => type;
    public TileEffect Effects => effects;
    public Sprite Sprite => sprite;
}
