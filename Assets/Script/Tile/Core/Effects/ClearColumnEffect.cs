using System.Collections.Generic;
using UnityEngine;

public class ClearColumnEffect : ITileEffect
{
    public HashSet<Vector2Int> ApplyEffect(List<List<Tile>> board, Vector2Int tilePosition)
    {
        HashSet<Vector2Int> tilesToDestroy = new HashSet<Vector2Int>();
        int column = tilePosition.x;
        for (int y = 0; y < board[column].Count; y++)
        {
            tilesToDestroy.Add(new Vector2Int(column, y));
        }
        return tilesToDestroy;
    }
}
