using System.Collections.Generic;
using UnityEngine;

public interface ITileEffect
{
    HashSet<Vector2Int> ApplyEffect(List<List<Tile>> board, Vector2Int tilePosition);
}
