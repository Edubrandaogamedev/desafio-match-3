using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTileSpawnStrategy : ISpecialTileSpawnStrategy 
{
    public abstract int Priority { get; }
    public abstract TileEffect TileEffect { get; }
    public abstract TileType TileType { get; set; }
    public abstract bool ShouldSpawnSpecialTile(List<List<Tile>> board, int matchSize, Dictionary<Vector2Int, Tile> matchedTiles, SwapDirection swapDirection);
    
    protected HashSet<Vector2Int> CheckHorizontalMatches(List<List<Tile>> board, Vector2Int position, int width)
    {
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();
        foreach (var horizontalDirections in new Vector2Int[] {Vector2Int.left, Vector2Int.right})
        {
            BoardService.SearchMatchesOnDirection(board, position, matches, horizontalDirections, width);
        }
        return matches;
    }
    protected HashSet<Vector2Int> CheckVerticalMatches(List<List<Tile>> board, Vector2Int position, int height)
    {
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();
        foreach (var verticalDirections in new Vector2Int[] {Vector2Int.up, Vector2Int.down})
        {
            BoardService.SearchMatchesOnDirection(board, position, matches, verticalDirections, height);
        }
        return matches;
    }
}
