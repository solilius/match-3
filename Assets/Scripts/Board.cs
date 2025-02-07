using UnityEngine;

public class Board : MonoBehaviour
{ 
    public TileSO[,] BoardGrid { get; private set; }
    
    [SerializeField] private GameObject tilePrefab;
    
    private TilesCatalog tileCatalog;
    
    public void Initialize(TilesCatalog catalog, TileSO[,] grid)
    {
        tileCatalog = catalog;
        SetBoard(grid);
    }
    
    private void SetBoard(TileSO[,] grid)
    {
        BoardGrid = grid;

        for (int x = 0; x < BoardGrid.GetLength(0); x++)
        {
            for (int y = 0; y < BoardGrid.GetLength(1); y++)
            {
                GameObject tile = Instantiate(tilePrefab, CalcTilePosition(x, y), Quaternion.identity);
                TileSO tileData = tileCatalog.GetTile(BoardGrid[x, y].id);
                tile.GetComponent<Tile>().Initialize(tileData, x, y);
            }
        }
    }
    
    private Vector3 CalcTilePosition(float x, float y)
    {
        // Make the spawn at top left instead of center
        // this way the area of the tile is (0-1, 0-1, 0) instead of (-0.5-0.5, -0.5-0.5, 0)
        return new Vector3(x, -y, 0f) + new Vector3(0.5f, -0.5f, 0f);
    }
}