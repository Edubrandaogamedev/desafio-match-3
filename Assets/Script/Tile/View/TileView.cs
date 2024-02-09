using UnityEngine;
using UnityEngine.UI;

public class TileView : MonoBehaviour
{
    [SerializeField] 
    private Image _image;

    public TileView Setup(Sprite sprite)
    {
        _image.sprite = sprite;
        return this;
    }
}