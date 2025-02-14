using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public static Dictionary<Vector2Int,Tile> GetTileInfoByPosition(List<List<Tile>> board, List<Vector2Int> positionSet)
    {
        Dictionary<Vector2Int,Tile> tileSet = new Dictionary<Vector2Int,Tile>();
        foreach (var position in positionSet)
        {
            tileSet[position] = board[position.y][position.x];
        }
        return tileSet;
    }
    
    public static void UpdateBoard(List<List<Tile>> newBoardTiles)
    {
        BoardTiles = newBoardTiles;
    }

    public static void SetTileAtSpecificPlace(List<List<Tile>> board, Vector2Int position, TileData data)
    {
        board[position.y][position.x].Setup(data,_tileCount++);
    }
    
    public static Vector2Int GetClosestFreeNeighborPosition(List<List<Tile>> board, Vector2Int position)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        int width = board[0].Count;
        int height = board.Count;

        queue.Enqueue(position);
        visited.Add(position);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (board[current.y][current.x].Id == -1)
            {
                return current;
            }

            foreach (var direction in new Vector2Int[] { Vector2Int.down, Vector2Int.right, Vector2Int.left, Vector2Int.up})
            {
                Vector2Int neighbor = current + direction;
                if (IsInsideBoard(neighbor, width,height) && !visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }
        return Vector2Int.zero;
    }
    
    public static void CleanTilesByPosition(List<List<Tile>> board,List<Vector2Int> matchTilePositions)
    {
        foreach (var tilePosition in matchTilePositions)
        {
            board[tilePosition.y][tilePosition.x] = new Tile();
        }
    }
    
    public static bool HasMatches(List<List<Tile>> boardToCheck, int matchSize, Vector2Int position)
    {
        int width = boardToCheck[0].Count;
        int height = boardToCheck.Count;
        HashSet<Vector2Int> horizontalGroup = new HashSet<Vector2Int>();
        HashSet<Vector2Int> verticalGroup = new HashSet<Vector2Int>();
        SearchMatchesOnDirection(boardToCheck, position,horizontalGroup,Vector2Int.right,width);
        SearchMatchesOnDirection(boardToCheck, position,horizontalGroup,Vector2Int.left,width);
        SearchMatchesOnDirection(boardToCheck,position,verticalGroup,Vector2Int.up,height);
        SearchMatchesOnDirection(boardToCheck,position,verticalGroup,Vector2Int.down,height);
        return horizontalGroup.Count >= matchSize || verticalGroup.Count >= matchSize;
    }
    
    public static HashSet<Vector2Int> GetMatchesPosition(List<List<Tile>> board, int matchSize)
    {
        HashSet<Vector2Int> matchedTilesPosition = new HashSet<Vector2Int>();
        int width = board[0].Count;
        int height = board.Count;
        bool[,] visited = new bool[height, width];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (visited[y, x])
                {
                    continue;
                }
                
                HashSet<Vector2Int> horizontalGroup = new HashSet<Vector2Int>();
                SearchMatchesOnDirection(board,new Vector2Int(x, y), horizontalGroup, Vector2Int.right,width,visited);
                SearchMatchesOnDirection(board,new Vector2Int(x, y), horizontalGroup, Vector2Int.left,width,visited);
                if (horizontalGroup.Count >= matchSize)
                {
                    matchedTilesPosition.UnionWith(horizontalGroup);
                }
                
                HashSet<Vector2Int> verticalGroup = new HashSet<Vector2Int>();
                SearchMatchesOnDirection(board,new Vector2Int(x, y), verticalGroup, Vector2Int.down,height,visited);
                SearchMatchesOnDirection(board,new Vector2Int(x, y), verticalGroup, Vector2Int.up,height,visited);
                if (verticalGroup.Count >= matchSize)
                {
                    matchedTilesPosition.UnionWith(verticalGroup);
                }
            }
        }
        matchedTilesPosition.ToList().Sort((a, b) =>
        {
            if (a.y != b.y) return a.y.CompareTo(b.y);
            return a.x.CompareTo(b.x);
        });
        return matchedTilesPosition;
    }
    

    public static List<MovedTileInfo> DropTiles(List<List<Tile>> board, List<Vector2Int> matchedTilesGroup, List<AddedTileInfo> exceptionTiles)
    {
        Dictionary<int, MovedTileInfo> movedTiles = new Dictionary<int, MovedTileInfo>();
        List<MovedTileInfo> movedTilesList = new List<MovedTileInfo>();
        foreach (var tilePosition in matchedTilesGroup)
        {
            if (IsExceptionTile(exceptionTiles, tilePosition.x, tilePosition.y))
            {
                continue;
            }

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
                if (board[y][x].Type == TileType.None)
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

    public static void SearchMatchesOnDirection(List<List<Tile>> boardToCheck, Vector2Int startPos, HashSet<Vector2Int> matchGroup, Vector2Int direction, int directionSize, bool[,] visited = null)
    {
        int width = boardToCheck[0].Count;
        int height = boardToCheck.Count;
        for (int i = 0; i < directionSize; i++)
        {
            int x = startPos.x + i * direction.x;
            int y = startPos.y + i * direction.y;

            if (!IsInsideBoard(new Vector2Int(x,y),width,height))
            {
                break;
            }
                
            if (boardToCheck[y][x].Type != boardToCheck[startPos.y][startPos.x].Type)
            {
                break;
            }
            matchGroup.Add(new Vector2Int(x, y));
            if (visited != null)
            {
                visited[y, x] = true;
            }
        }
    }
    
    private static bool IsInsideBoard(Vector2Int position, int boardWidth, int boardHeight)
    {
        return position.x >= 0 && position.x < boardWidth && position.y >= 0 && position.y < boardHeight;
    }
    
    private static bool IsExceptionTile(List<AddedTileInfo> exceptionTiles, int x, int y)
    {
        foreach (var exceptionTile in exceptionTiles)
        {
            if (exceptionTile.position.x == x && exceptionTile.position.y == y)
            {
                return true;
            }
        }
        return false;
    }
    
    private static MovedTileInfo MoveTile(List<List<Tile>> board, int x, int y, Dictionary<int, MovedTileInfo> movedTiles)
    {
        MovedTileInfo movedTileInfo = null;
        Tile movedTile = board[y - 1][x];
        board[y][x] = movedTile;
        if (movedTile.Type != TileType.None)
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
        
        if (x > 1 && board[y][x - 1].Type == board[y][x - 2].Type)
        {
            noMatchTypes.Remove(board[y][x - 1].Data);
        }
                
        if (y > 1 && board[y - 1][x].Type == board[y - 2][x].Type)
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