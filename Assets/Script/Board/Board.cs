using System.Collections.Generic;
using UnityEngine;

public static class Board
{
    private static TileData[] _tilesData;
    private static int _tileCount;

    public static List<List<Tile>> BoardTiles { get; private set; }

    public static List<List<Tile>> Initialize(int width, int height, TileData[] tilesData)
    {
        _tilesData = tilesData;
        BoardTiles = InitializeBoard(width, height);
        SetupBoard(BoardTiles);
        return BoardTiles;
    }
    
    public static List<List<Tile>> CopyBoardTiles(bool self = true, List<List<Tile>> boardTilesToCopy = null)
    {
        return self ? 
            BoardTiles.ConvertAll(row => row.ConvertAll(tile => new Tile().Setup(tile.Data, tile.Id))) :
            boardTilesToCopy.ConvertAll(row => row.ConvertAll(tile => new Tile().Setup(tile.Data, tile.Id)));
    }
    
    public static void SetNewBoardTile(List<List<Tile>> newBoardTiles)
    {
        BoardTiles = newBoardTiles;
    }

    public static bool CheckMatches(List<List<Tile>> boardToCheck, int x, int y)
    {
        var hasHorizontalMatch = x > 1 && boardToCheck[y][x].Key == boardToCheck[y][x - 1].Key && boardToCheck[y][x - 1].Key == boardToCheck[y][x - 2].Key;
        var hasVerticalMatch = y > 1 && boardToCheck[y][x].Key == boardToCheck[y - 1][x].Key && boardToCheck[y - 1][x].Key == boardToCheck[y - 2][x].Key;
        return hasHorizontalMatch || hasVerticalMatch;
    }
    
    public static HashSet<Vector2Int> GetMatchesPosition(bool self,List<List<Tile>> boardToCheck = null)
    {
        return self ? GetMatchesPosition(BoardTiles) : GetMatchesPosition(boardToCheck);
    }

    private static HashSet<Vector2Int> GetMatchesPosition(List<List<Tile>> boardToCheck = null)
    {
        HashSet<Vector2Int> matchesPositions = new HashSet<Vector2Int>();
        for (int y = 0; y < boardToCheck.Count; y++)
        {
            for (int x = 0; x < boardToCheck[y].Count; x++)
            {
                if (x > 1 && boardToCheck[y][x].Key == boardToCheck[y][x - 1].Key && boardToCheck[y][x - 1].Key == boardToCheck[y][x - 2].Key)
                {
                    matchesPositions.Add(new Vector2Int(x,y));
                    matchesPositions.Add(new Vector2Int(x-1,y));
                    matchesPositions.Add(new Vector2Int(x-2,y));
                }
                if (y > 1 && boardToCheck[y][x].Key == boardToCheck[y - 1][x].Key && boardToCheck[y - 1][x].Key == boardToCheck[y - 2][x].Key)
                {
                    matchesPositions.Add(new Vector2Int(x,y));
                    matchesPositions.Add(new Vector2Int(x,y-1));
                    matchesPositions.Add(new Vector2Int(x,y-2));
                }
            }
        }
        return matchesPositions;
    }
    
    private static List<List<Tile>> InitializeBoard(int width, int height)
    {
        var board = new List<List<Tile>>(height);
        for (int y = 0; y < height; y++)
        {
            var row = new List<Tile>(width);
            for (int x = 0; x < width; x++)
            {
                row.Add(new Tile());
            }
            board.Add(row);
        }
        return board;
    }
    
    private static void SetupBoard(List<List<Tile>> board)
    {
        _tileCount = 0;
        for (int y = 0; y < board.Count; y++)
        {
            for (int x = 0; x < board[y].Count; x++)
            {
                List<TileData> noMatchTypes = GetAvailableTypes(board,x,y);
                board[y][x].Setup(noMatchTypes[Random.Range(0, noMatchTypes.Count)],_tileCount++);
            }
        }
    }
    
    private static List<TileData> GetAvailableTypes(List<List<Tile>> board, int x, int y)
    {
        List<TileData> noMatchTypes = new List<TileData>(_tilesData);
        
        if (x > 1 && board[y][x - 1].Key == board[y][x - 2].Key)
        {
            noMatchTypes.Remove(board[y][x - 1].Data);
        }
                
        if (y > 1 && board[y - 1][x].Key == board[y - 2][x].Key)
        {
            noMatchTypes.Remove(board[y - 1][x].Data);
        }
        return noMatchTypes;
    }
}