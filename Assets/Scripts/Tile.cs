using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileSprite;
    [SerializeField] private TMP_Text textPos;

    private int _x;
    private int _y;
    
    private bool _isSwapping;

    void OnEnable()
    {
        TileEvents.OnUpdateTilePosition += MoveTile;
        MatchHandler.OnMatched += OnMatch;
    }

    void OnDisable()
    {
        TileEvents.OnUpdateTilePosition -= MoveTile;
        MatchHandler.OnMatched -= OnMatch;
    }

    public void Initialize(TileSO data, int x, int y)
    {
        SetPosition(x, y);
        if (tileSprite != null && data.sprite != null)
        {
            tileSprite.sprite = data.sprite;
        }
    }

    private void MoveTile(object sender, UpdateTilePositionEventArgs e)
    {
        if (e.GameObjectId == gameObject.GetInstanceID())
        {
            Vector3 targetPosition = CalcTilePosition(e.NewPosition);
            transform.DOMove(targetPosition, e.Duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                SetPosition(e.NewPosition.x, e.NewPosition.y);
            });
        }
    }

    private void OnMatch(object sender, MatchedTileEventArgs e)
    {
        if (e.GameObjectId == gameObject.GetInstanceID())
        {
            transform.DOScale(0f, e.Duration).OnComplete(() => { Destroy(gameObject); });
        }
    }

    private void SetPosition(int x, int y)
    {
        _x = x;
        _y = y;
        gameObject.name = $"Tile ({_x}, {_y})";
        textPos.text = $"{_x},{_y}";
    }
    
    private static Vector3 CalcTilePosition(Vector2Int position)
    {
        // Make the spawn at top left instead of center
        // this way the area of the tile is (0-1, 0-1, 0) instead of (-0.5-0.5, -0.5-0.5, 0)
        return new Vector3(position.x, position.y, 0f) + new Vector3(0.5f, -0.5f, 0f);
    }
}