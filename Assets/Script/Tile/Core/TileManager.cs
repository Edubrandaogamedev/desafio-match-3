using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    TileDataCollection _dataCollection;
    
    private readonly Dictionary<TileEffect, List<TileData>> dataByEffect = new Dictionary<TileEffect, List<TileData>>();
    private readonly List<ISpecialTileSpawnStrategy> specialTileSpawnStrategies = new List<ISpecialTileSpawnStrategy>();

    private void Awake()
    {
        ProcessDataByEffect();
        PopulateSpecialTileStrategies();
    }
    
    private void ProcessDataByEffect()
    {
        foreach (TileData data in _dataCollection.TilesData)
        {
            if (!dataByEffect.ContainsKey(data.Effects))
            {
                dataByEffect[data.Effects] = new List<TileData>();
            }
            dataByEffect[data.Effects].Add(data);
        }
    }

    private void PopulateSpecialTileStrategies()
    {
        specialTileSpawnStrategies.Add(new ClearRowTileSpawnStrategy());
        specialTileSpawnStrategies.Add(new ClearColumnTileSpawnStrategy());
    }

    public List<TileData> GetTileDataCollectionByEffect(TileEffect effect)
    {
        return dataByEffect[effect];
    }

    public TileData GetRandomTileDataByEffect(TileEffect effect)
    {
        int randomIndex = Random.Range(0, dataByEffect[effect].Count);
        return dataByEffect[effect][randomIndex];
    }

    public TileData? GetTileDataByEffectAndKey(TileType type, TileEffect effect)
    {
        foreach (var tileData in dataByEffect[effect])
        {
            if (tileData.Type == type)
            {
                return tileData;
            }
        }
        return null;
    }
    
    public List<TileData> CheckForSpecialTilesByPriority(List<List<Tile>> board,int matchSize, Dictionary<Vector2Int,Tile> tilesPositionSet, SwapDirection swapDirection)
    {
        List<TileData> specialTileData = new List<TileData>();
        foreach (var strategy in specialTileSpawnStrategies)
        {

            if (!strategy.ShouldSpawnSpecialTile(board,matchSize,tilesPositionSet,swapDirection))
            {
                continue;
            }
            specialTileData.Add(GetTileDataByEffectAndKey(strategy.TileType, strategy.TileEffect).Value);
        }
        return specialTileData;
    }
}
