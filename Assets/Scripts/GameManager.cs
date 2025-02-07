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
        TileSO[,] grid = new TileSO[6, 6];
        string[] tileIds = { "Banana", "Apple", "Grape", "Pear" };

        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 6; col++)
            {
                string randomId = tileIds[UnityEngine.Random.Range(0, tileIds.Length)];
                grid[row, col] = tileCatalog.GetTile(randomId);
            }
        }

        return grid;
    }
}
