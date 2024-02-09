using System.Collections.Generic;
using UnityEngine;

public class ClearRowTileSpawnStrategy : ISpecialTileSpawnStrategy
{
    public int Priority => 1;
    public TileEffect TileEffect => TileEffect.Special_Clear_Row;
    public bool ShouldSpawnSpecialTile(List<Vector2Int> matchedTilesPositions,SwapDirection swapDirection)
    {
        return matchedTilesPositions.Count >= 4 && swapDirection == SwapDirection.Horizontal;
    }
}