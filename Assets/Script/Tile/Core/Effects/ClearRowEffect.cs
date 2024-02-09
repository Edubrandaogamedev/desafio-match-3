using System.Collections.Generic;
using UnityEngine;

public class ClearRowEffect : ITileEffect
{
    public HashSet<Vector2Int> ApplyEffect(List<List<Tile>> board, Vector2Int tilePosition)
    {
        Debug.Log($"<color=cyan> TO DESTRUINDO TUDO EM LINHA AAAAAA </color>");
        return GetAllTilesInRow(board,tilePosition);
    }

    private HashSet<Vector2Int> GetAllTilesInRow(List<List<Tile>> board,Vector2Int tilePosition)
    {
        HashSet<Vector2Int> tilesToDestroy = new HashSet<Vector2Int>();
        int row = tilePosition.y;
        for (int x = 0; x < board[row].Count; x++)
        {
            tilesToDestroy.Add(new Vector2Int(x, row));
        }
        return tilesToDestroy;
    }
}
