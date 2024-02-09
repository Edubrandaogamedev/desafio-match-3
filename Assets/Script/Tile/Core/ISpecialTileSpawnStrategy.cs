using System.Collections.Generic;
using UnityEngine;

public interface ISpecialTileSpawnStrategy
{
    bool ShouldSpawnSpecialTile(List<Vector2Int> matchedTiles,SwapDirection swapDirection);
    int Priority { get; }
    TileEffect TileEffect { get; }
}