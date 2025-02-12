using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private BoardManager _boardManager;
    private TileGenerator _tileGenerator;
    private TilesCatalog _tileCatalog;

    private void Awake()
    {
        _tileCatalog = gameObject.AddComponent<TilesCatalog>();
    }

    void Start()
    {
        _boardManager = FindFirstObjectByType<BoardManager>();
        _tileGenerator = FindFirstObjectByType<TileGenerator>();
        Initialize();
    }

    private void Initialize()
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
        
        _tileGenerator.Initialize(_tileCatalog, procGenTile);
        _boardManager.Initialize(size, procGenBoard);
    }
}