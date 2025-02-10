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
    private bool _isPlayMode;
    
    void Awake()
    {
        _isPlayMode = Application.isPlaying;
        textPos.gameObject.SetActive(!_isPlayMode);
    }
    
    void OnEnable()
    {
        TileEvents.OnUpdateTilePosition += MoveTile;
        BoardManager.OnMatched += OnMatch;
    }

    void OnDisable()
    {
        TileEvents.OnUpdateTilePosition -= MoveTile;
        BoardManager.OnMatched -= OnMatch;
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
            Vector3 targetPosition = BoardManager.CalcTilePosition(e.NewPosition);
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

        if (!_isPlayMode)
        {
            textPos.text = $"{_x},{_y}";
        }
    }
}