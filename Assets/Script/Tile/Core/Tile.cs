using UnityEngine;

public class Tile
{
    private TileData _data;
    public TileData Data => _data;
    public string Key => _data.Key;
    public Vector2Int Position { get; private set; } = new Vector2Int(-1, 1);
    public int Id { get; private set; } = -1;

    public Tile Setup(TileData data, Vector2Int position, int id)
    {
        _data = data;
        Position = position;
        Id = id;
        return this;
    }
}
