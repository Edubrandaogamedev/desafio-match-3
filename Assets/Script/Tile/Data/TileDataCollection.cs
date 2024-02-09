using UnityEngine;

[CreateAssetMenu(fileName = "TileDataCollection", menuName = "Gameplay/TileDataCollection")]
public class TileDataCollection : ScriptableObject
{
    [SerializeField] private TileData[] tilesDatas;
    
    public TileData[] TilesData => tilesDatas;
}