using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController
{
    private TileManager _tileManager;
    private int _currentScore;
    private int matchSize = 3;
    public Action<int> OnScoreUpdated;

    public GameController(TileManager tileManager)
    {
        _tileManager = tileManager;
    }
    
    public bool IsValidMovement(int fromX, int fromY, int toX, int toY)
    {
        List<List<Tile>> swappedBoard = BoardService.SwapTile(fromX, fromY, toX, toY);
        bool hasMatchOnFromPosition = BoardService.HasMatches(swappedBoard, matchSize, new Vector2Int(fromX, fromY));
        bool hasMatchOnToPosition = BoardService.HasMatches(swappedBoard, matchSize, new Vector2Int(toX, toY));
        return hasMatchOnFromPosition || hasMatchOnToPosition;
    }

    //TODO: Need flow review, for now I locked the dynamically special tile creation to avoid some position and sequence visual bugs, need review
    public List<BoardSequence> ProcessMatches(int fromX, int fromY, int toX, int toY)
    {
        List<List<Tile>> swappedBoard = BoardService.SwapTile(fromX, fromY, toX, toY);
        List<BoardSequence> boardSequences = new List<BoardSequence>();
        List<Vector2Int> tilesToCheckPosition = new List<Vector2Int>() { new Vector2Int(fromX, fromY), new Vector2Int(toX, toY) };
        List<TileData> tileData = GetSpecialTileDataToCreate(swappedBoard, tilesToCheckPosition, new Vector2Int(fromX, fromY), new Vector2Int(toX, toY));
        while (ProcessingMatches(swappedBoard,out List<Vector2Int> matchedTiles))
        {
            matchedTiles = ActivateTileEffect(swappedBoard,matchedTiles);
            BoardService.CleanTilesByPosition(swappedBoard,matchedTiles);
            List<AddedTileInfo> specialTiles = ProcessSpecialTile(swappedBoard,tileData,tilesToCheckPosition, new Vector2Int(fromX,fromY),new Vector2Int(toX,toY));
            List<MovedTileInfo> movedTilesList = BoardService.DropTiles(swappedBoard,matchedTiles,specialTiles);
            List<AddedTileInfo> addedTiles = BoardService.FillEmptySpaces(swappedBoard);
            IncreaseScore(matchedTiles.Count);
            
            BoardSequence sequence = new BoardSequence
            {
                matchedPosition = matchedTiles,
                movedTiles = movedTilesList,
                addedTiles = addedTiles,
                specialTiles =  specialTiles,
            };
            boardSequences.Add(sequence);
        }
        BoardService.UpdateBoard(swappedBoard);
        return boardSequences;
    }

    private List<TileData> GetSpecialTileDataToCreate(List<List<Tile>> board, List<Vector2Int> tilesToCheck, Vector2Int from, Vector2Int to)
    {
        Dictionary<Vector2Int, Tile> tileSet = BoardService.GetTileInfoByPosition(board,tilesToCheck);
        SwapDirection swapDirection = (to - from).x == 0 ? SwapDirection.Vertical : SwapDirection.Horizontal;
        return _tileManager.CheckForSpecialTilesByPriority(board,matchSize,tileSet,swapDirection);
    }
    
    private bool ProcessingMatches(List<List<Tile>> board, out List<Vector2Int> matchedTiles)
    {
        matchedTiles = BoardService.GetMatchesPosition(board,matchSize).ToList();
        Debug.Log("<color=yellow> ------------------------------------------------ </color>");
        foreach (var group in matchedTiles)
        {
            Debug.Log($"<color=orange> GROUP ELEMENT: {group} </color>");
        }
        Debug.Log("<color=yellow> ------------------------------------------------ </color>");
        return matchedTiles.Count > 0;
    }

    private List<AddedTileInfo> ProcessSpecialTile(List<List<Tile>> board,List<TileData> specialTileData, List<Vector2Int> matchedTiles,  Vector2Int from, Vector2Int to)
    {
        List<AddedTileInfo> addedTileInfos = new List<AddedTileInfo>();
        foreach (var tileData in specialTileData)
        {
            Vector2Int specialTilePosition = default;
            if (matchedTiles.Contains(from))
            {
                specialTilePosition = board[from.y][from.x].Id == -1 ? from : BoardService.GetClosestFreeNeighborPosition(board, from);
            }
            else if (matchedTiles.Contains(to))
            {
                specialTilePosition = board[to.y][to.x].Id == -1 ? to : BoardService.GetClosestFreeNeighborPosition(board, to);
            }
            BoardService.SetTileAtSpecificPlace(board, specialTilePosition, tileData);
            addedTileInfos.Add(new AddedTileInfo
            {
                position = specialTilePosition,
                data = tileData
            });
        }
        specialTileData.Clear();
        return addedTileInfos;
    }
    
    
    private void IncreaseScore(int value)
    {
        _currentScore += value;
        OnScoreUpdated?.Invoke(_currentScore);
    }

    private List<Vector2Int> ActivateTileEffect(List<List<Tile>> board, List<Vector2Int> matchedTilesPosition)
    {
        HashSet<Vector2Int> affectedTiles = new HashSet<Vector2Int>();
        foreach (var tilePosition in matchedTilesPosition)
        {
            HashSet<Vector2Int> tiles = board[tilePosition.y][tilePosition.x].ApplyEffect(board, tilePosition);
            affectedTiles.UnionWith(tiles);
        }

        if (affectedTiles.Count <= 0)
        {
            return matchedTilesPosition;
        }
        
        affectedTiles.UnionWith(matchedTilesPosition);
        List<Vector2Int> sortedAffectedTiles = affectedTiles.ToList();
        sortedAffectedTiles.Sort((a, b) =>
        {
            if (a.y != b.y) return a.y.CompareTo(b.y);
            return a.x.CompareTo(b.x);
        });
        return sortedAffectedTiles;
    }
}
