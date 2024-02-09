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

    public List<BoardSequence> ProcessMatches(int fromX, int fromY, int toX, int toY)
    {
        List<List<Tile>> swappedBoard = BoardService.SwapTile(fromX, fromY, toX, toY);
        List<BoardSequence> boardSequences = new List<BoardSequence>();
        while (ProcessingMatches(swappedBoard,out List<List<Vector2Int>> matchedTilesGroups))
        {
            List<Vector2Int> matchedTilesList = ConvertMatchTilesGroupToList(matchedTilesGroups);
            List<TileType> groupsTileType = GetGroupsTileType(swappedBoard,matchedTilesGroups);
            matchedTilesList =  ActivateTileEffect(swappedBoard,matchedTilesList);
            BoardService.CleanTilesByPosition(swappedBoard,matchedTilesList);
            List<AddedTileInfo> specialTiles = ProcessSpecialTile(swappedBoard,groupsTileType,matchedTilesGroups, new Vector2Int(fromX,fromY),new Vector2Int(toX,toY));
            List<MovedTileInfo> movedTilesList = BoardService.DropTiles(swappedBoard,matchedTilesList,specialTiles);
            List<AddedTileInfo> addedTiles = BoardService.FillEmptySpaces(swappedBoard);
            IncreaseScore(matchedTilesList.Count);
            
            BoardSequence sequence = new BoardSequence
            {
                matchedPosition = matchedTilesList,
                movedTiles = movedTilesList,
                addedTiles = addedTiles,
                specialTiles =  specialTiles,
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

    private List<AddedTileInfo> ProcessSpecialTile(List<List<Tile>> board,List<TileType> groupTileTypes, List<List<Vector2Int>> matchedTilesGroups,  Vector2Int from, Vector2Int to)
    {
        List<AddedTileInfo> addedTileInfos = new List<AddedTileInfo>();
        for (int i = 0; i < matchedTilesGroups.Count; i++)
        {
            SwapDirection swapDirection = (to - from).x == 0 ? SwapDirection.Vertical : SwapDirection.Horizontal;
            TileData? specialTileData = _tileManager.CheckForSpecialTilesByPriority(matchedTilesGroups[i], groupTileTypes[i],swapDirection);
            if (specialTileData != null)
            {
                Vector2Int specialTilePosition = default;
                if (matchedTilesGroups[i].Contains(from))
                {
                    specialTilePosition = board[from.y][from.x].Id == -1 ? from : BoardService.GetClosestFreeNeighborPosition(board, from);
                }
                else if (matchedTilesGroups[i].Contains(to))
                {
                    specialTilePosition = board[to.y][to.x].Id == -1 ? to : BoardService.GetClosestFreeNeighborPosition(board, to);
                }
                BoardService.SetTileAtSpecificPlace(board, specialTilePosition, specialTileData.Value);
                addedTileInfos.Add(new AddedTileInfo
                {
                    position = specialTilePosition,
                    data = specialTileData.Value
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

    private List<Vector2Int> ActivateTileEffect(List<List<Tile>> board, List<Vector2Int> matchedTilesPosition)
    {
        HashSet<Vector2Int> affectedTiles = new HashSet<Vector2Int>();
        foreach (var tilePosition in matchedTilesPosition)
        {
            HashSet<Vector2Int> tiles = board[tilePosition.y][tilePosition.x].ApplyEffect(board, tilePosition);
            affectedTiles.UnionWith(tiles);
        }
        affectedTiles.UnionWith(matchedTilesPosition);
        List<Vector2Int> sortedAffectedTiles = affectedTiles.ToList();
        sortedAffectedTiles.Sort((a, b) =>
        {
            if (a.y != b.y) return a.y.CompareTo(b.y); // Ordena primeiro por y
            return a.x.CompareTo(b.x); // Se y for o mesmo, ordena por x
        });
        return sortedAffectedTiles;
    }
    private List<TileType> GetGroupsTileType(List<List<Tile>> board, List<List<Vector2Int>> matchedTilesGroups)
    {
        List<TileType> tileTypes = new List<TileType>();
        foreach (var group in matchedTilesGroups)
        {
            Vector2Int tilePosition = group[0];
            TileType groupType = board[tilePosition.y][tilePosition.x].Type;
            tileTypes.Add(groupType);
        }
        return tileTypes;
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
