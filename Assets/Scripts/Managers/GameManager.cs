using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int boardSize= 6;
    [SerializeField] private AssetReferenceGameObject tilesList;
    [SerializeField] private ProcGenMapSO procGenTile;
    [SerializeField] private ProcGenMapSO procGenBoard;
    [SerializeField] private int scoreToPassLevel = 1000;
    [SerializeField] private float scoreToPassLevelModifier = 1.5f;

    private BoardManager _boardManager;
    private TileGenerator _tileGenerator;
    private TilesCatalog _tileCatalog;
    private ScoreManager _scoreManager;

    private int _level = 1;
    
    private void Awake()
    {
        _tileCatalog = gameObject.AddComponent<TilesCatalog>();
        tilesList.LoadAssetAsync().Completed += LoadAddressables;
    }

    void Start()
    {
        _scoreManager = FindFirstObjectByType<ScoreManager>();
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

        _scoreManager.StartLevel(Mathf.FloorToInt(scoreToPassLevel));
    }
}