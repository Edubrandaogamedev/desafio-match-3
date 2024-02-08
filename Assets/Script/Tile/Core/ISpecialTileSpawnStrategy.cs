using System.Collections.Generic;
using UnityEngine;

public interface ISpecialTileSpawnStrategy
{
    bool ShouldSpawnSpecialTile(HashSet<Tile> matchedTiles);
    int Priority { get; }
    TileEffect TileEffect { get; }
}