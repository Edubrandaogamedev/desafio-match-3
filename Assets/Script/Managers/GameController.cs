using System;
using System.Collections.Generic;
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

    public List<BoardSequence> ProcessMatches(int fromX, int fromY, int toX, int toY)
    {
        List<List<Tile>> swappedBoard = BoardService.SwapTile(fromX, fromY, toX, toY);
        List<BoardSequence> boardSequences = new List<BoardSequence>();
        while (ProcessingMatches(swappedBoard,out List<List<Vector2Int>> matchedTilesGroups))
        {
            List<Vector2Int> matchedTilesList = ConvertMatchTilesGroupToList(matchedTilesGroups);
            Dictionary<int, TileData> specialTileData = GetSpecialTileDataByMatchGroup(swappedBoard, matchedTilesGroups);
            
            BoardService.CleanTilesByPosition(swappedBoard,matchedTilesList);
            List<AddedTileInfo> specialTiles = ProcessSpecialTile(swappedBoard,specialTileData,matchedTilesGroups, new Vector2Int(fromX,fromY),new Vector2Int(toX,toY));
            List<MovedTileInfo> movedTilesList = BoardService.DropTiles(swappedBoard,matchedTilesList,specialTiles);
            List<AddedTileInfo> addedTiles = BoardService.FillEmptySpaces(swappedBoard);
            IncreaseScore(matchedTilesGroups.Count);
            
            BoardSequence sequence = new BoardSequence
            {
                matchedPosition = matchedTilesList,
                movedTiles = movedTilesList,
                addedTiles = addedTiles,
                specialTiles =  specialTiles
            };
            boardSequences.Add(sequence);
        }
        BoardService.UpdateBoard(swappedBoard);
        return boardSequences;
    }
    
    private bool ProcessingMatches(List<List<Tile>> board, out List<List<Vector2Int>> matchedTileGroups)
    {
        matchedTileGroups = BoardService.GetMatchesGroups(board,matchSize);
        HashSet<Vector2Int> test = BoardService.GetMatchesPosition(false, board);
        int index = 0;
        foreach (var group in matchedTileGroups)
        {
            index++;
            Debug.Log($"<color=red> GROUP {index} </color>");
            foreach (var gElement in group)
            {
                Debug.Log($"<color=orange> GROUP ELEMENT: {gElement} </color>");
            }
        }
        Debug.Log("------------------------------------------------");
        foreach (var position in test)
        {
            Debug.Log($"<color=orange> POSITION: {position} </color>");
        }
        Debug.Log("<color=yellow> ------------------------------------------------ </color>");
        return matchedTileGroups.Count > 0;
    }

    private List<AddedTileInfo> ProcessSpecialTile(List<List<Tile>> board,Dictionary<int, TileData> specialTileData, List<List<Vector2Int>> matchedTilesGroups,  Vector2Int from, Vector2Int to)
    {
        List<AddedTileInfo> addedTileInfos = new List<AddedTileInfo>();
        for (int i = 0; i < matchedTilesGroups.Count; i++)
        {
            Vector2Int specialTilePosition;
            if (matchedTilesGroups[i].Contains(from) && specialTileData.ContainsKey(i))
            {
                specialTilePosition = board[from.y][from.x].Id == -1 ? from : BoardService.GetClosestFreeNeighborPosition(board, from);
                BoardService.SetTileAtSpecificPlace(board,specialTilePosition,specialTileData[i]);
                addedTileInfos.Add(new AddedTileInfo
                {
                    position = specialTilePosition,
                    data = specialTileData[i]
                });
            }
            else if (matchedTilesGroups[i].Contains(to) && specialTileData.ContainsKey(i))
            {
                specialTilePosition = board[to.y][to.x].Id == -1 ? to : BoardService.GetClosestFreeNeighborPosition(board, to);
                BoardService.SetTileAtSpecificPlace(board,specialTilePosition,specialTileData[i]);
                addedTileInfos.Add(new AddedTileInfo
                {
                    position = specialTilePosition,
                    data = specialTileData[i]
                });
            }
        }
        return addedTileInfos;
    }
    
    
    private void IncreaseScore(int value)
    {
        _currentScore += value;
        OnScoreUpdated?.Invoke(_currentScore);
    }

    private Dictionary<int,TileData> GetSpecialTileDataByMatchGroup(List<List<Tile>> board, List<List<Vector2Int>> matchedTilesGroups)
    {
        Dictionary<int,TileData> tileDataByGroup = new Dictionary<int,TileData>();
        for (var i = 0; i < matchedTilesGroups.Count; i++)
        {
            List<Vector2Int> group = matchedTilesGroups[i];
            TileType groupType = board[group[i].y][group[i].x].Type;
            TileData? specialTileData = _tileManager.CheckForSpecialTilesByPriority(group, groupType);
            if (specialTileData != null)
            {
                tileDataByGroup[i] = specialTileData.Value;
            }
        }
        return tileDataByGroup;
    }
    
    private List<Vector2Int> ConvertMatchTilesGroupToList(List<List<Vector2Int>> matchedTilesGroups)
    {
        List<Vector2Int> matchTilesPositions = new List<Vector2Int>();
        foreach (var group in matchedTilesGroups)
        {
            foreach (var tilePosition in group)
            {
                if (!matchTilesPositions.Contains(tilePosition))
                {
                    matchTilesPositions.Add(tilePosition);
                }
            }
        }
        return matchTilesPositions;
    }
}
