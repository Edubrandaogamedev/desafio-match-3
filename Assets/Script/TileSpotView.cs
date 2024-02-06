using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TileSpotView : MonoBehaviour
{
    [SerializeField] private Button _button;

    private int _x;
    private int _y;

    public event Action<int, int> onClick;

    private void Awake()
    {
        _button.onClick.AddListener(OnTileClick);
    }

    private void OnTileClick()
    {
        onClick?.Invoke(_x, _y);
    }

    public void SetPosition(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public void SetTileView(TileView tile)
    {
        tile.transform.SetParent(transform, false);
        tile.transform.position = transform.position;
    }

    public Tween AnimatedSetTile(TileView tile)
    {
        tile.transform.SetParent(transform);
        tile.transform.DOKill();
        return tile.transform.DOMove(transform.position, 0.3f);
    }

}