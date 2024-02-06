using System.Collections.Generic;
using UnityEngine;

public class GameController
{
    private List<List<Tile>> _boardTiles;
    private TileData[] _tilesData;
    private int _tileCount;

    public List<List<Tile>> StartGame(int boardWidth, int boardHeight, TileData[] tilesData)
    {
        _tilesData = tilesData;
        _boardTiles = CreateBoard(boardWidth, boardHeight);
        return _boardTiles;
    }

    public bool IsValidMovement(int fromX, int fromY, int toX, int toY)
    {
        List<List<Tile>> newBoard = CopyBoard(_boardTiles);

        (newBoard[fromY][fromX], newBoard[toY][toX]) = (newBoard[toY][toX], newBoard[fromY][fromX]);

        for (int y = 0; y < newBoard.Count; y++)
        {
            for (int x = 0; x < newBoard[y].Count; x++)
            {
                if (x > 1
                    && newBoard[y][x].Key == newBoard[y][x - 1].Key
                    && newBoard[y][x - 1].Key == newBoard[y][x - 2].Key)
                {
                    return true;
                }
                if (y > 1
                    && newBoard[y][x].Key == newBoard[y - 1][x].Key
                    && newBoard[y - 1][x].Key == newBoard[y - 2][x].Key)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public List<BoardSequence> SwapTile(int fromX, int fromY, int toX, int toY)
    {
        List<List<Tile>> newBoard = CopyBoard(_boardTiles);

        (newBoard[fromY][fromX], newBoard[toY][toX]) = (newBoard[toY][toX], newBoard[fromY][fromX]);

        List<BoardSequence> boardSequences = new List<BoardSequence>();
        List<List<bool>> matchedTiles;
        while (HasMatch(matchedTiles = FindMatches(newBoard)))
        {
            //Cleaning the matched tiles
            List<Vector2Int> matchedPosition = new List<Vector2Int>();
            for (int y = 0; y < newBoard.Count; y++)
            {
                for (int x = 0; x < newBoard[y].Count; x++)
                {
                    if (matchedTiles[y][x])
                    {
                        matchedPosition.Add(new Vector2Int(x, y));
                        newBoard[y][x] = new Tile();
                    }
                }
            }

            // Dropping the tiles
            Dictionary<int, MovedTileInfo> movedTiles = new Dictionary<int, MovedTileInfo>();
            List<MovedTileInfo> movedTilesList = new List<MovedTileInfo>();
            for (int i = 0; i < matchedPosition.Count; i++)
            {
                int x = matchedPosition[i].x;
                int y = matchedPosition[i].y;
                if (y > 0)
                {
                    for (int j = y; j > 0; j--)
                    {
                        Tile movedTile = newBoard[j - 1][x];
                        newBoard[j][x] = movedTile;
                        if (movedTile.Key != null)
                        {
                            if (movedTiles.ContainsKey(movedTile.Id))
                            {
                                movedTiles[movedTile.Id].to = new Vector2Int(x, j);
                            }
                            else
                            {
                                MovedTileInfo movedTileInfo = new MovedTileInfo
                                {
                                    from = new Vector2Int(x, j - 1),
                                    to = new Vector2Int(x, j)
                                };
                                movedTiles.Add(movedTile.Id, movedTileInfo);
                                movedTilesList.Add(movedTileInfo);
                            }
                        }
                    }

                    newBoard[0][x] = new Tile();
                }
            }

            // Filling the board
            List<AddedTileInfo> addedTiles = new List<AddedTileInfo>();
            for (int y = newBoard.Count - 1; y > -1; y--)
            {
                for (int x = newBoard[y].Count - 1; x > -1; x--)
                {
                    if (newBoard[y][x].Key == null)
                    {
                        int tileType = Random.Range(0, _tilesData.Length);
                        Tile tile = newBoard[y][x];
                        tile.Setup(_tilesData[tileType], _tileCount++);
                        addedTiles.Add(new AddedTileInfo
                        {
                            position = new Vector2Int(x, y),
                            key = tile.Key,
                            data = tile.Data
                        });
                    }
                }
            }

            BoardSequence sequence = new BoardSequence
            {
                matchedPosition = matchedPosition,
                movedTiles = movedTilesList,
                addedTiles = addedTiles
            };
            boardSequences.Add(sequence);
        }

        _boardTiles = newBoard;
        return boardSequences;
        //return _boardTiles;
    }

    private static bool HasMatch(List<List<bool>> list)
    {
        for (int y = 0; y < list.Count; y++)
            for (int x = 0; x < list[y].Count; x++)
                if (list[y][x])
                    return true;
        return false;
    }

    private static List<List<bool>> FindMatches(List<List<Tile>> newBoard)
    {
        List<List<bool>> matchedTiles = new List<List<bool>>();
        for (int y = 0; y < newBoard.Count; y++)
        {
            matchedTiles.Add(new List<bool>(newBoard[y].Count));
            for (int x = 0; x < newBoard.Count; x++)
            {
                matchedTiles[y].Add(false);
            }
        }

        for (int y = 0; y < newBoard.Count; y++)
        {
            for (int x = 0; x < newBoard[y].Count; x++)
            {
                if (x > 1
                    && newBoard[y][x].Key == newBoard[y][x - 1].Key
                    && newBoard[y][x - 1].Key == newBoard[y][x - 2].Key)
                {
                    matchedTiles[y][x] = true;
                    matchedTiles[y][x - 1] = true;
                    matchedTiles[y][x - 2] = true;
                }
                if (y > 1
                    && newBoard[y][x].Key == newBoard[y - 1][x].Key
                    && newBoard[y - 1][x].Key == newBoard[y - 2][x].Key)
                {
                    matchedTiles[y][x] = true;
                    matchedTiles[y - 1][x] = true;
                    matchedTiles[y - 2][x] = true;
                }
            }
        }

        return matchedTiles;
    }

    private static List<List<Tile>> CopyBoard(List<List<Tile>> boardToCopy)
    {
        List<List<Tile>> newBoard = new List<List<Tile>>(boardToCopy.Count);
        for (int y = 0; y < boardToCopy.Count; y++)
        {
            newBoard.Add(new List<Tile>(boardToCopy[y].Count));
            for (int x = 0; x < boardToCopy[y].Count; x++)
            {
                Tile tile = boardToCopy[y][x];
                newBoard[y].Add(new Tile().Setup(tile.Data,tile.Id));
            }
        }

        return newBoard;
    }

    private List<List<Tile>> CreateBoard(int width, int height)
    {
        List<List<Tile>> board = new List<List<Tile>>(height);
        _tileCount = 0;
        for (int y = 0; y < height; y++)
        {
            board.Add(new List<Tile>(width));
            for (int x = 0; x < width; x++)
            {
                board[y].Add(new Tile());
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                List<TileData> noMatchTypes = new List<TileData>(_tilesData);
                if (x > 1
                    && board[y][x - 1].Key == board[y][x - 2].Key)
                {
                    noMatchTypes.Remove(board[y][x - 1].Data);
                }
                if (y > 1
                    && board[y - 1][x].Key == board[y - 2][x].Key)
                {
                    noMatchTypes.Remove(board[y - 1][x].Data);
                }

                board[y][x].Setup(noMatchTypes[Random.Range(0, noMatchTypes.Count)], _tileCount++);
            }
        }

        return board;
    }
}
