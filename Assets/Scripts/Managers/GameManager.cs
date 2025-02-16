using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class GameManager : MonoBehaviour
{
    [SerializeField] private int boardSize= 6;
    [SerializeField] private AssetReferenceGameObject tilesList;
    [SerializeField] private ProcGenMapSO procGenTile;
    [SerializeField] private ProcGenMapSO procGenBoard;
    [SerializeField] private int baseLevelScore = 1000;
    [SerializeField] private int scoreLevelModifier = 100;
    [SerializeField] private int maxMoves = 30;

    private BoardManager _boardManager;
    private TileGenerator _tileGenerator;
    private TilesCatalog _tileCatalog;
    private ScoreManager _scoreManager;
    private PopupManager _popupManager;

    private int _level;
    private int _moves;
    
    private void Awake()
    {
        _tileCatalog = gameObject.AddComponent<TilesCatalog>();
        tilesList.LoadAssetAsync().Completed += LoadAddressables;
        _moves = maxMoves;
        _level = 1;
    }

    void Start()
    {
        _scoreManager = FindFirstObjectByType<ScoreManager>();
        _popupManager = FindFirstObjectByType<PopupManager>();
        _boardManager = FindFirstObjectByType<BoardManager>();
        _tileGenerator = FindFirstObjectByType<TileGenerator>();
    }
    
    private void LoadAddressables(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            List<TileSO> tiles = handle.Result.GetComponent<AddressableTilesList>().TilesList;
            Initialize(tiles);
        }
        else
        {
            Debug.LogError($"Failed to load asset: {nameof(LoadAddressables)}");
        }
    }

    private void Initialize(List<TileSO> tilesScriptableObjects)
    {
        _tileCatalog.Initialize(tilesScriptableObjects);
        _tileGenerator.Initialize(_tileCatalog, procGenTile.procGenRules);
        _boardManager.Initialize(boardSize, procGenBoard.procGenRules);
        StartLevel();
    }

    private void StartLevel()
    {
        int scoreCompleteLevel = baseLevelScore + (_level * scoreLevelModifier);
        _scoreManager.StartLevel(Mathf.FloorToInt(scoreCompleteLevel));
        OnMoveUsed?.Invoke(this, _moves);
    }
}