using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileSprite;
    [SerializeField] private TMP_Text textPos;
    [SerializeField] private float moveCooldownTime = 0.5f;

    private string id;
    private int _x;
    private int _y;

    private bool _isSwapping;
    [SerializeField] private  float _moveCooldown = 0;

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

    void Update()
    {
        _moveCooldown -= Time.deltaTime;
    }

    public void Initialize(TileSO data, int x, int y)
    {
        SetPosition(x, y);
        id = data.variant;
        if (tileSprite != null && data.sprite != null)
        {
            tileSprite.sprite = data.sprite;
        }
    }

    private void MoveTile(object sender, UpdateTilePositionEventArgs e)
    {
        if (e.Position.x == _x && e.Position.y == _y && _moveCooldown <= 0)
        {
            _moveCooldown = moveCooldownTime;
            Debug.Log(e.Position);
            Vector3 targetPosition = transform.position + new Vector3(e.Direction.x, e.Direction.y, 0f);

            transform.DOMove(targetPosition, e.Duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                SetPosition(_x + e.Direction.x, _y + e.Direction.y);
            });
        }
    }

    private void OnMatch(object sender, MatchedTileEventArgs e)
    {
        if (e.Position.x == _x && e.Position.y == _y)
        {
            _moveCooldown = 0;
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
}