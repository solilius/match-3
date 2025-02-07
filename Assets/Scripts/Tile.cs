using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileSprite;

    private TileSO _data;
    private int _x;
    private int _y;

    public void Initialize(TileSO data, int x, int y)
    {
        _data = data;
        _x = x;
        _y = y;
        gameObject.name = $"Tile ({_x}, {_y})";

        if (tileSprite != null && data.sprite != null)
        {
            tileSprite.sprite = data.sprite;
        }
    }
}