using System.Collections.Generic;
using UnityEngine;

public class ClearRowTileSpawnStrategy : ISpecialTileSpawnStrategy
{
    public int Priority => 1;
    public TileEffect TileEffect => TileEffect.Special_Clear_Row;
    public bool ShouldSpawnSpecialTile(List<Vector2Int> matchedTilesPositions)
    {
        return matchedTilesPositions.Count >= 4 && CheckDirection(matchedTilesPositions);
    }
    
    private bool CheckDirection(List<Vector2Int> matchedTilesPositions)
    {
        int firstY = matchedTilesPositions[0].y;
        foreach (var tile in matchedTilesPositions)
        {
            if (tile.y != firstY)
            {
                return false;
            }
        }
        return true;
    }
}


