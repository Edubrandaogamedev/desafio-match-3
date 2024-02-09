using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClearColumnTileSpawnStrategy : BaseTileSpawnStrategy
{
    public override int Priority => 1;
    public override TileEffect TileEffect => TileEffect.Special_Clear_Column;
    public override TileType TileType { get; set; } = TileType.None;

    public override bool ShouldSpawnSpecialTile(List<List<Tile>> board,int matchSize, Dictionary<Vector2Int,Tile> matchedTiles, SwapDirection swapDirection)
    {
        TileType = TileType.None;
        bool isVerticalSwap = swapDirection == SwapDirection.Vertical;
        foreach (var tilePosition in matchedTiles.Keys)
        {
            if (isVerticalSwap && CheckConditional(board, tilePosition, matchSize+1))
            {
                TileType = matchedTiles[tilePosition].Type;
                return true;
            }
        }
        return false;
    }

    private bool CheckConditional(List<List<Tile>> board, Vector2Int tilePosition, int matchSize)
    {
        int width = board[0].Count;
        int height = board.Count;
        HashSet<Vector2Int> horizontalMatches = CheckHorizontalMatches(board, tilePosition, width);
        if (horizontalMatches.Count > 0 && HasConsecutiveMatches(board,horizontalMatches.ToList(), matchSize))
        {
            return true;
        }
        HashSet<Vector2Int> verticalMatches = CheckVerticalMatches(board, tilePosition, height);
        if (verticalMatches.Count > 0 && HasConsecutiveMatches(board,verticalMatches.ToList(), matchSize))
        {
            return true;
        }
        return false;
    }
    
    private bool HasConsecutiveMatches(List<List<Tile>> board, List<Vector2Int> matches, int matchSize)
    {
        Vector2Int firstTileToCheck = matches[0];
        TileType targetType = board[firstTileToCheck.y][firstTileToCheck.x].Type;
        int consecutiveMatches = 0;
        foreach (var matchPosition in matches)
        {
            if (board[matchPosition.y][matchPosition.x].Type == targetType)
            {
                consecutiveMatches++;
                if (consecutiveMatches >= matchSize)
                {
                    return true;
                }
            }
            else
            {
                targetType = board[matchPosition.y][matchPosition.x].Type;
                consecutiveMatches = 0;
            }
        }
        return false;
    }
}