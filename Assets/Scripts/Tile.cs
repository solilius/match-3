using DG.Tweening;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileSprite;
    [SerializeField] private float swapDuration = .5f;

    private TileSO _data;
    private float _x;
    private float _y;

    private bool _isSwapping;

    void OnEnable()
    {
        SwapTiles.OnSwitchPosition += MoveTile;
    }

    void OnDisable()
    {
        SwapTiles.OnSwitchPosition -= MoveTile;
    }

    public void Initialize(TileSO data, int x, int y)
    {
        _data = data;
        SetPosition(x, y);

        if (tileSprite != null && data.sprite != null)
        {
            tileSprite.sprite = data.sprite;
        }
    }

    private void MoveTile(Vector2 from, Vector2 direction)
    {
        if (!_isSwapping && from.x == _x && from.y == _y)
        {
            _isSwapping = true;
            Vector3 targetPosition = transform.position + new Vector3(direction.x, -direction.y, 0f);
            
            transform.DOMove(targetPosition, swapDuration).OnComplete(() =>
            {
                _isSwapping = false;
                SetPosition(_x + direction.x, _y + direction.y);
            });
        }
    }

    private void SetPosition(float x, float y)
    {
        _x = x;
        _y = y;
        gameObject.name = $"Tile ({_x}, {_y})";
    }
}