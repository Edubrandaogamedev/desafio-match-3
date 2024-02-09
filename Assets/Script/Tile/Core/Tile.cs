using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public TileData Data => _data;
    public string Key { get; private set; }
    public TileType Type { get; private set; } = TileType.None;
    public TileEffect Effects { get; private set; } = TileEffect.Default;
    public int Id { get; private set; } = -1;

    private TileData _data;
    private readonly TileEffectFactory _effectFactory = new TileEffectFactory();
    private readonly List<ITileEffect> _effects = new List<ITileEffect>();

    public Tile Setup(TileData data, int id)
    {
        _data = data;
        Id = id;
        Key = _data.Key;
        Type = _data.Type;
        Effects = _data.Effects;
        SetEffects();
        return this;
    }

    public HashSet<Vector2Int> ApplyEffect(List<List<Tile>> board, Vector2Int tilePosition)
    {
        HashSet<Vector2Int> affectedTiles = new HashSet<Vector2Int>();
        foreach (var effect in _effects)
        {
            Debug.Log($"<color=cyan> {effect} </color>");
            affectedTiles.UnionWith(effect.ApplyEffect(board,tilePosition));
        }
        return affectedTiles;
    }

    private void SetEffects()
    {
        foreach (TileEffect effectType in Enum.GetValues(typeof(TileEffect)))
        {
            if (!Effects.HasFlag(effectType))
            {
                continue;
            }
            
            ITileEffect effect = _effectFactory.GetTileEffect(effectType);
            if (effect != null)
            {
                _effects.Add(effect);
            }
        }
    }
}
