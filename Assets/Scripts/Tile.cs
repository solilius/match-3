using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileSprite;
    [SerializeField] private float swapSpeed = 10f;

    private TileSO _data;
    private float _x;
    private float _y;
    
    private bool _isSwaping;
    
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
        if (!_isSwaping && from.x == _x && from.y == _y)
        {
            _isSwaping = true;
            
            Vector2 targetPosition = transform.position + new Vector3(direction.x, -direction.y, 0f);
            StartCoroutine(MoveToDirection(targetPosition));
            SetPosition(_x + direction.x, _y + direction.y);
        }
    }

    private IEnumerator MoveToDirection(Vector2 target)
    {
        Vector2 startPosition = transform.position;
        float distance = Vector2.Distance(startPosition, target);
        float startTime = Time.time;

        while (Vector2.Distance(transform.position, target) > 0.01f)
        {
            float elapsedTime = Time.time - startTime;
            float fraction = elapsedTime * swapSpeed / distance;
            transform.position = Vector2.Lerp(startPosition, target, fraction);
            yield return null;
        }

        transform.position = target;
        _isSwaping = false;
    }

    private void SetPosition(float x, float y)
    {
        _x = x;
        _y = y;
        gameObject.name = $"Tile ({_x}, {_y})";
    }
}