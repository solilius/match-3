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
        _tileGenerator = gameObject.AddComponent<TileGenerator>();
        _boardManager = FindFirstObjectByType<BoardManager>();
    }

    void Start()
    {
        _tileGenerator.Initialize(_tileCatalog, new Dictionary<GenLogic, float> { { GenLogic.Random, 1 } });
        Dictionary<GenLogic, float> tileGenMap = new Dictionary<GenLogic, float> { { GenLogic.Random, 1 } };
        _boardManager.Initialize(_tileGenerator, _tileGenerator.GenerateBoard(6, 6, tileGenMap));
    }

    // private string[,] TempInit()
    // {
    //     int rows = 6;
    //     int cols = 6;
    //     string[,] grid = new string[rows, cols];
    //     string[] tileVariants = { "Banana", "Apple", "Pear", "Grape" };
    //
    //     for (int row = 0; row < rows; row++)
    //     {
    //         for (int col = 0; col < cols; col++)
    //         {
    //             string randomVariant = tileVariants[UnityEngine.Random.Range(0, tileVariants.Length)];
    //             grid[row, col] = randomVariant;
    //         }
    //     }
    //
    //     return grid;
    // }

    private string[,] TempInit()
    {
        string[,] grid = new string[6, 6]
        {
            { "Banana", "Grape", "Apple", "Pear", "Grape", "Apple" },
            { "Banana", "Grape", "Apple", "Pear", "Banana", "Apple" },
            { "Pear", "Pear", "Banana", "Apple", "Pear", "Grape" },
            { "Apple", "Apple", "Pear", "Banana", "Apple", "Pear" },
            { "Banana", "Grape", "Apple", "Pear", "Banana", "Apple" },
            { "Pear", "Banana", "Pear", "Apple", "Pear", "Banana" }
        };

        return grid;
    }
}