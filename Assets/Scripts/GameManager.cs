using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    private BoardManager _boardManager;
    
    private TilesCatalog tileCatalog;
    
    private void Awake()
    { 
        tileCatalog = gameObject.AddComponent<TilesCatalog>();
        _boardManager = FindFirstObjectByType<BoardManager>();
    }

    void Start()
    {
        _boardManager.Initialize(tileCatalog, TempInit());
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
            { "Pear", "Banana", "Grape", "Apple", "Pear", "Banana" }
        };

        return grid;
    }
}
