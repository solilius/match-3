using DG.Tweening;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileSprite;
    [SerializeField] private AudioClip popSound;
    [SerializeField] private AudioClip swapSound;
    [SerializeField] private AudioClip swapBackSound;
    [SerializeField] private TMP_Text textPos;
    [SerializeField] private ParticleSystem popEffect;

    private int _x;
    private int _y;

    private bool _isSwapping;
    private bool _isPlayMode;

    private AudioSource _audioSource;

    void Awake()
    {
        _isPlayMode = Application.isPlaying;
        textPos.gameObject.SetActive(!_isPlayMode);
        _audioSource = GetComponent<AudioSource>();
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
        tileSprite.sprite = data.sprite;
        SetPosition(x, y);
        SetEffectColor(data.popEffectColor);
    }

    private void MoveTile(object sender, UpdateTilePositionEventArgs e)
    {
        if (e.GameObjectId != gameObject.GetInstanceID()) return;

        if (e.IsSwap && e.IsSwapBack) PlaySound(swapBackSound);
        else if (e.IsSwap) PlaySound(swapSound);

        Vector3 targetPosition = BoardManager.CalcTilePosition(e.NewPosition);
        transform.DOMove(targetPosition, e.Duration).SetEase(Ease.Linear)
            .OnComplete(() => SetPosition(e.NewPosition.x, e.NewPosition.y));
    }

    private void OnMatch(object sender, MatchedTileEventArgs e)
    {
        if (e.GameObjectId == gameObject.GetInstanceID())
        {
            PlaySound(popSound); // move to sound manager (have delay of 1ms so sounds won't overlap)
            popEffect.Play();
            transform.DOScale(0f, e.Duration).OnComplete(() =>
            {
                var main = popEffect.main;
                Destroy(gameObject, main.duration);
            });
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

    private void SetEffectColor(Color color)
    {
        var mainModule = popEffect.main;
        mainModule.startColor = new ParticleSystem.MinMaxGradient(color);
    }

    private void PlaySound(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}