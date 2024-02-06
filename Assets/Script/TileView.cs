using UnityEngine;
using UnityEngine.UI;

public class TileView : MonoBehaviour
{
    [SerializeField] 
    private Image _image;

    public TileView Setup(Color colorType)
    {
        _image.color = colorType;
        return this;
    }
}