using System;
using UnityEngine;


[Serializable]
public struct TileData
{
    [SerializeField]
    private string key;
    //TODO instead of using color change this to sprite
    [SerializeField]
    private Color type;

    public string Key => key;
    public Color Type => type;
}
