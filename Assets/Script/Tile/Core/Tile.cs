using UnityEngine;

public class Tile
{
    private TileData _data;
    public TileData Data => _data;
    public string Key => _data.Key;
    public TileType Type { get; private set; } = TileType.None;
    public int Id { get; private set; } = -1;

    public Tile Setup(TileData data, int id)
    {
        _data = data;
        Type = _data.Type;
        Id = id;
        return this;
    }
}
