using System;
using UnityEngine;

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
