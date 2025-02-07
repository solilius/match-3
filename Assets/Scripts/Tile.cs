using DG.Tweening;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileSprite;
    [SerializeField] private float swapDuration = .5f;

    private int _x;
    private int _y;

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
        SetPosition(x, y);

        if (tileSprite != null && data.sprite != null)
        {
            tileSprite.sprite = data.sprite;
        }
    }

    private void MoveTile(object sender, SwitchPositionEventArgs e)
    {
        if (!_isSwapping && e.Position.x == _x && e.Position.y == _y)
        {
            _isSwapping = true;
            Vector3 targetPosition = transform.position + new Vector3(e.Direction.x, -e.Direction.y, 0f);

            transform.DOMove(targetPosition, swapDuration).OnComplete(() =>
            {
                _isSwapping = false;
                SetPosition(_x + e.Direction.x, _y + e.Direction.y);
            });
        }
    }

    private void SetPosition(int x, int y)
    {
        _x = x;
        _y = y;
        gameObject.name = $"Tile ({_x}, {_y})";
    }
}