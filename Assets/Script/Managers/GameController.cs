using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController
{
    private List<List<Tile>> _boardTiles;
    private int _tileCount;
    private int _currentScore;

    public Action<int> OnScoreUpdated;

    public List<List<Tile>> StartGame(int boardWidth, int boardHeight, TileData[] tilesData)
    {
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
            List<AddedTileInfo> addedTiles = BoardService.FillEmptySpaces(swappedBoard);
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
    }
    
    private bool ProcessingMatches(List<List<Tile>> board, out HashSet<Vector2Int> matchedTilesPosition)
    {
        matchedTilesPosition = BoardService.GetMatchesPosition(self: false, board);
        return matchedTilesPosition.Count > 0;
    }
    
    private void IncreaseScore(int value)
    {
        _currentScore += value;
        OnScoreUpdated?.Invoke(_currentScore);
    }
}
