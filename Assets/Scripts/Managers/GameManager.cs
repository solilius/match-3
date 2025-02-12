using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject tilesList;

    private BoardManager _boardManager;
    private TileGenerator _tileGenerator;
    private TilesCatalog _tileCatalog;

    private void Awake()
    {
        tilesList.LoadAssetAsync().Completed += LoadAddressables;
        _tileCatalog = gameObject.AddComponent<TilesCatalog>();
    }

    void Start()
    {
        _boardManager = FindFirstObjectByType<BoardManager>();
        _tileGenerator = FindFirstObjectByType<TileGenerator>();
        _tileCatalog = gameObject.AddComponent<TilesCatalog>();
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
        int size = 6;
        Dictionary<GenLogic, float> procGenTile = new Dictionary<GenLogic, float>
        {
            { GenLogic.RandomPower, 0.05f },
            { GenLogic.Match3, 0.1f },
            { GenLogic.Match2, 0.2f },
            { GenLogic.RandomFruit, 0.3f },
            { GenLogic.NoMatch, 0.45f },
        };

        Dictionary<GenLogic, float> procGenBoard = new Dictionary<GenLogic, float>
        {
            { GenLogic.RandomPower, 0.15f },
            { GenLogic.NoMatch, 0.55f },
            { GenLogic.Match2, 0.3f },
        };

        _tileCatalog.Initialize(tilesScriptableObjects);
        _tileGenerator.Initialize(_tileCatalog, procGenTile);
        _boardManager.Initialize(size, procGenBoard);
    }
}