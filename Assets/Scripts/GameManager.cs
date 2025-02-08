using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] private Board board;
    
    private TilesCatalog tileCatalog;
    
    private void Awake()
    { 
        tileCatalog = gameObject.AddComponent<TilesCatalog>();
    }

    void Start()
    {
        board.Initialize(tileCatalog, TempInit());
    }
    
    private TileSO[,] TempInit()
    {
        int rows = 6;
        int cols = 6;
        TileSO[,] grid = new TileSO[rows, cols];
        string[] tileIds = { "Banana", "Apple", "Pear", "Grape" };

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                string randomId = tileIds[UnityEngine.Random.Range(0, tileIds.Length)];
                grid[row, col] = tileCatalog.GetTile(randomId);
            }
        }

        return grid;
    }
}
