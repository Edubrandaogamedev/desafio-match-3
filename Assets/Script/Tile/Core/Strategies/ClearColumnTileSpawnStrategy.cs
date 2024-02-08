using System.Collections.Generic;
using UnityEngine;

public class ClearColumnTileSpawnStrategy : ISpecialTileSpawnStrategy
{
    public int Priority => 1;
    public TileEffect TileEffect => TileEffect.Special_Clear_Column;
    public bool ShouldSpawnSpecialTile(List<Vector2Int> matchedTilesPositions)
    {
        return matchedTilesPositions.Count >= 4 && CheckDirection(matchedTilesPositions);
    }

    private bool CheckDirection(List<Vector2Int> matchedTiles)
    {
        int firstX = matchedTiles[0].x;
        foreach (var tile in matchedTiles)
        {
            if (tile.x != firstX)
            {
                return false;
            }
        }
        return true;
    }
}