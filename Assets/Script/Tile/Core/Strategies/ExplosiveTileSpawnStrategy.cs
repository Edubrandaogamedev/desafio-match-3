using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveTileSpawnStrategy : BaseTileSpawnStrategy
{
    public override int Priority => 2;
    public override TileEffect TileEffect => TileEffect.Special_Clear_Area;
    public override TileType TileType { get; set; }

    public override bool ShouldSpawnSpecialTile(List<List<Tile>> board,int matchSize, Dictionary<Vector2Int,Tile> matchedTiles, SwapDirection swapDirection)
    {
        return CheckConditional(board,matchedTiles,matchSize);
    }
    
    private bool CheckConditional(List<List<Tile>> board, Dictionary<Vector2Int,Tile> matchedTiles, int matchSize)
    {
        TileType = TileType.None;
        int width = board[0].Count;
        int height = board.Count;
        foreach (var position in matchedTiles.Keys)
        {
            HashSet<Vector2Int> horizontalMatches = CheckHorizontalMatches(board, position, width);
            HashSet<Vector2Int> verticalMatches = CheckVerticalMatches(board, position, height);
            if (horizontalMatches.Count >= matchSize && verticalMatches.Count >= matchSize)
            {
                TileType = matchedTiles[position].Type;
                return true;
            }
        }
        return false;
    }
}
