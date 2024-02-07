using System.Collections.Generic;
using UnityEngine;

public static class BoardService
{
    private static TileManager _tileManager;
    private static int _tileCount;

    public static List<List<Tile>> BoardTiles { get; private set; }

    public static List<List<Tile>> Initialize(int width, int height, TileManager tileManager)
    {
        _tileManager = tileManager;
        BoardTiles = InitializeBoard(width, height);
        return BoardTiles;
    }

    public static List<List<Tile>> SwapTile(int fromX, int fromY, int toX, int toY)
    {
        List<List<Tile>> newBoard = CopyBoardTiles();
        (newBoard[fromY][fromX], newBoard[toY][toX]) = (newBoard[toY][toX], newBoard[fromY][fromX]);
        return newBoard;
    }
    
    public static void UpdateBoard(List<List<Tile>> newBoardTiles)
    {
        BoardTiles = newBoardTiles;
    }

    public static void CleanTilesByPosition(List<List<Tile>> board,HashSet<Vector2Int> tilesPosition)
    {
        foreach (var tilePosition in tilesPosition)
        {
            board[tilePosition.y][tilePosition.x] = new Tile();
        }
    }
    
    public static bool HasMatches(List<List<Tile>> boardToCheck)
    {
        for (int y = 0; y < boardToCheck.Count; y++)
        {
            for (int x = 0; x < boardToCheck[y].Count; x++)
            {
                var hasHorizontalMatch = x > 1 && boardToCheck[y][x].Key == boardToCheck[y][x - 1].Key && boardToCheck[y][x - 1].Key == boardToCheck[y][x - 2].Key;
                var hasVerticalMatch = y > 1 && boardToCheck[y][x].Key == boardToCheck[y - 1][x].Key && boardToCheck[y - 1][x].Key == boardToCheck[y - 2][x].Key;
                if (hasHorizontalMatch || hasVerticalMatch)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    public static HashSet<Vector2Int> GetMatchesPosition(bool self,List<List<Tile>> boardToCheck = null)
    {
        return self ? GetMatchesPosition(BoardTiles) : GetMatchesPosition(boardToCheck);
    }

    public static List<MovedTileInfo> DropTiles(List<List<Tile>> board, HashSet<Vector2Int> matchedTilesPosition)
    {
        Dictionary<int, MovedTileInfo> movedTiles = new Dictionary<int, MovedTileInfo>();
        List<MovedTileInfo> movedTilesList = new List<MovedTileInfo>();
        
        foreach (var tilePosition in matchedTilesPosition)
        {
            const int bottomRow = 0;
            int x = tilePosition.x;
            int y = tilePosition.y;
            if (y <= bottomRow)
            {
                continue;
            }
            
            for (int j = y; j > bottomRow; j--)
            {
                MovedTileInfo movedTileInfo = MoveTile(board, x, j, movedTiles);
                if (movedTileInfo != null)
                {
                    movedTilesList.Add(MoveTile(board, x, j, movedTiles));
                }
            }
            board[0][x] = new Tile();
        }
        return movedTilesList;
    }
    
    public static List<AddedTileInfo> FillEmptySpaces(List<List<Tile>> board)
    {
        List<AddedTileInfo> addedTiles = new List<AddedTileInfo>();

        for (int y = board.Count - 1; y >= 0; y--)
        {
            for (int x = board[y].Count - 1; x >= 0; x--)
            {
                if (board[y][x].Key == null)
                {
                    TileData tileData = _tileManager.GetRandomTileDataByEffect(TileEffect.Default);
                    Tile tile = new Tile();
                    tile.Setup(tileData, _tileCount++);
                    board[y][x] = tile;

                    addedTiles.Add(new AddedTileInfo
                    {
                        position = new Vector2Int(x, y),
                        data = tile.Data
                    });
                }
            }
        }
        return addedTiles;
    }

    private static HashSet<Vector2Int> GetMatchesPosition(List<List<Tile>> boardToCheck = null)
    {
        HashSet<Vector2Int> matchesPositions = new HashSet<Vector2Int>();
        for (int y = 0; y < boardToCheck.Count; y++)
        {
            for (int x = 0; x < boardToCheck[y].Count; x++)
            {
                //Keep the position in this order, to avoid bugging the DoTween Sequence
                if (x > 1 && boardToCheck[y][x].Key == boardToCheck[y][x - 1].Key && boardToCheck[y][x - 1].Key == boardToCheck[y][x - 2].Key)
                {
                    matchesPositions.Add(new Vector2Int(x-2,y));
                    matchesPositions.Add(new Vector2Int(x-1,y));
                    matchesPositions.Add(new Vector2Int(x,y));
                }
                if (y > 1 && boardToCheck[y][x].Key == boardToCheck[y - 1][x].Key && boardToCheck[y - 1][x].Key == boardToCheck[y - 2][x].Key)
                {
                    matchesPositions.Add(new Vector2Int(x,y-2));
                    matchesPositions.Add(new Vector2Int(x,y-1));
                    matchesPositions.Add(new Vector2Int(x,y));
                }
            }
        }
        return matchesPositions;
    }
    
    private static MovedTileInfo MoveTile(List<List<Tile>> board, int x, int y, Dictionary<int, MovedTileInfo> movedTiles)
    {
        MovedTileInfo movedTileInfo = null;
        Tile movedTile = board[y - 1][x];
        board[y][x] = movedTile;
        if (movedTile.Key != null)
        {
            if (movedTiles.TryGetValue(movedTile.Id, out movedTileInfo))
            {
                movedTileInfo.to = new Vector2Int(x, y);
            }
            else
            {
                movedTileInfo = new MovedTileInfo
                {
                    from = new Vector2Int(x, y - 1),
                    to = new Vector2Int(x, y)
                };
                movedTiles.Add(movedTile.Id, movedTileInfo);
            }
        }
        return movedTileInfo;
    }
    
    private static List<List<Tile>> InitializeBoard(int width, int height)
    {
        _tileCount = 0;
        var board = new List<List<Tile>>(height);
        for (int y = 0; y < height; y++)
        {
            board.Add(new List<Tile>(width));
            for (int x = 0; x < width; x++)
            {
                List<TileData> noMatchTypes = GetAvailableTypes(board,x,y);
                TileData randomType = noMatchTypes[Random.Range(0, noMatchTypes.Count)];
                board[y].Add(new Tile().Setup(randomType,_tileCount++));
            }
            
        }
        return board;
    }
    
    private static List<TileData> GetAvailableTypes(List<List<Tile>> board, int x, int y)
    {
        List<TileData> noMatchTypes = new List<TileData>(_tileManager.GetTileDataCollectionByEffect(TileEffect.Default));
        
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
    
    //TODO move it to an extension class
    private static List<List<Tile>> CopyBoardTiles(bool self = true, List<List<Tile>> boardTilesToCopy = null)
    {
        return self ? 
            BoardTiles.ConvertAll(row => row.ConvertAll(tile => new Tile().Setup(tile.Data, tile.Id))) :
            boardTilesToCopy.ConvertAll(row => row.ConvertAll(tile => new Tile().Setup(tile.Data, tile.Id)));
    }
}