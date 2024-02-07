using UnityEngine;

public class Tile
{
    private TileData _data;
    public TileData Data => _data;
    public string Key => _data.Key;
    public Color ViewAsset => _data.Type;
    public int Id { get; private set; } = -1;

    public Tile Setup(TileData data, int id)
    {
        _data = data;
        Id = id;
        return this;
    }
}
