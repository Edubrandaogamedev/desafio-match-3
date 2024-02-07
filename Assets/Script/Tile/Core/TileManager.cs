using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    TileDataCollection _dataCollection;
    
    private Dictionary<TileEffect, List<TileData>> dataByEffect = new Dictionary<TileEffect, List<TileData>>();

    private void Awake()
    {
        ProcessDataByEffect();
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

    public List<TileData> GetTileDataCollectionByEffect(TileEffect effect)
    {
        return dataByEffect[effect];
    }

    public TileData GetRandomTileDataByEffect(TileEffect effect)
    {
        int randomIndex = Random.Range(0, dataByEffect[effect].Count);
        return dataByEffect[effect][randomIndex];
    }
}
