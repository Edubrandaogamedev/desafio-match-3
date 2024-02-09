using System.Collections.Generic;
using UnityEngine;

public interface ISpecialTileSpawnStrategy
{
    bool ShouldSpawnSpecialTile(List<List<Tile>> board,int matchSize, Dictionary<Vector2Int,Tile> matchedTiles, SwapDirection swapDirection);
    int Priority { get; }
    TileEffect TileEffect { get; }
    TileType TileType { get; }
}