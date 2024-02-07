using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController
{
    private List<List<Tile>> _boardTiles;
    private TileData[] _tilesData;
    private int _tileCount;
    private int _currentScore;

    public Action<int> OnScoreUpdated;

    public List<List<Tile>> StartGame(int boardWidth, int boardHeight, TileData[] tilesData)
    {
        _tilesData = tilesData;
        return BoardService.Initialize(boardWidth, boardHeight, tilesData);;
    }

    public bool IsValidMovement(int fromX, int fromY, int toX, int toY)
    {
        List<List<Tile>> swappedBoard = BoardService.SwapTile(fromX, fromY, toX, toY);
        return BoardService.HasMatches(swappedBoard);
    }

    public List<BoardSequence> ProcessMatches(int fromX, int fromY, int toX, int toY)
    {
        List<List<Tile>> swappedBoard = BoardService.SwapTile(fromX, fromY, toX, toY);
        List<BoardSequence> boardSequences = new List<BoardSequence>();
        while (ProcessingMatches(swappedBoard,out HashSet<Vector2Int> matchedTilesPosition))
        {
            BoardService.CleanTilesByPosition(swappedBoard,matchedTilesPosition);
            IncreaseScore(matchedTilesPosition.Count);
            List<MovedTileInfo> movedTilesList = BoardService.DropTiles(swappedBoard,matchedTilesPosition);
            
            // Filling the board
            List<AddedTileInfo> addedTiles = new List<AddedTileInfo>();
            for (int y = swappedBoard.Count - 1; y > -1; y--)
            {
                for (int x = swappedBoard[y].Count - 1; x > -1; x--)
                {
                    if (swappedBoard[y][x].Key == null)
                    {
                        int tileType = Random.Range(0, _tilesData.Length);
                        Tile tile = swappedBoard[y][x];
                        tile.Setup(_tilesData[tileType], BoardService.IncreaseTileCount());
                        addedTiles.Add(new AddedTileInfo
                        {
                            position = new Vector2Int(x, y),
                            data = tile.Data
                        });
                    }
                }
            }

            BoardSequence sequence = new BoardSequence
            {
                matchedPosition = matchedTilesPosition.ToList(),
                movedTiles = movedTilesList,
                addedTiles = addedTiles
            };
            boardSequences.Add(sequence);
        }

        BoardService.SetNewBoardTile(swappedBoard);
        return boardSequences;
        //return _boardTiles;
    }
    
    private bool ProcessingMatches(List<List<Tile>> board, out HashSet<Vector2Int> matchedTilesPosition)
    {
        matchedTilesPosition = BoardService.GetMatchesPosition(self: false, board);
        return matchedTilesPosition.Count > 0;
    }
    
    // Method to fill empty spaces with new tiles
    private List<AddedTileInfo> FillEmptySpaces(List<List<Tile>> board)
    {
        List<AddedTileInfo> addedTiles = new List<AddedTileInfo>();

        for (int y = board.Count - 1; y >= 0; y--)
        {
            for (int x = board[y].Count - 1; x >= 0; x--)
            {
                if (board[y][x].Key == null)
                {
                    int tileType = Random.Range(0, _tilesData.Length);
                    Tile tile = new Tile();
                    tile.Setup(_tilesData[tileType], BoardService.IncreaseTileCount());
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
    
    private void IncreaseScore(int value)
    {
        _currentScore += value;
        OnScoreUpdated?.Invoke(_currentScore);
    }
}
