using System.Collections.Generic;
using System.Linq;
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

    public TileData? GetTileDataByEffectAndKey(string key, TileEffect effect)
    {
        foreach (var tileData in dataByEffect[effect])
        {
            if (tileData.Key == key)
            {
                return tileData;
            }
        }
        return null;
    }
    
    public List<TileData> CheckForSpecialTilesByPriority(List<HashSet<Tile>> tileMatchGroups)
    {
        List<TileData> tileData = new List<TileData>();
        int highestPriority = -1;
        foreach (var group in tileMatchGroups)
        {
            foreach (var strategy in specialTileSpawnStrategies)
            {
                if (strategy.Priority < highestPriority)
                {
                    continue;
                }

                if (!strategy.ShouldSpawnSpecialTile(group))
                {
                    continue;
                }
                
                if (strategy.Priority > highestPriority)
                {
                    tileData.Clear();
                }
                highestPriority = strategy.Priority;
                TileData? data = GetTileDataByEffectAndKey(group.First().Key, strategy.TileEffect); //wont be null and if happens, it's better to have an error than keep it silenced dealing with null
                tileData.Add(data.Value);
            }
        }
        return tileData;
    }
}
