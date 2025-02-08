using UnityEngine;

public class Board : MonoBehaviour
{ 
    public TileSO[,] BoardGrid { get; private set; }
    
    [SerializeField] private GameObject tilePrefab;
    
    private TilesCatalog _tileCatalog;
    
    public void Initialize(TilesCatalog catalog, TileSO[,] grid)
    {
        _tileCatalog = catalog;
        SetBoard(grid);
    }

    public bool IsInGrid(int x, int y)
    {
        return BoardGrid != null &&
               x >= 0 && x < BoardGrid.GetLength(0) &&
               y >= 0 && y < BoardGrid.GetLength(1);
    }

    public void SwitchTiles(Vector2Int tileA, Vector2Int tileB)
    {
        (BoardGrid[tileA.x, tileA.y], BoardGrid[tileB.x, tileB.y]) = 
            (BoardGrid[tileB.x, tileB.y], BoardGrid[tileA.x, tileA.y]);
    }

    public void RemoveTile(Vector2Int tile)
    {
        BoardGrid[tile.x, tile.y] = null;
    }
    
    public int GetScore(Vector2Int tile)
    {
        return ((FruitSO)BoardGrid[tile.x, tile.y])?.score ?? 0;
    }
    
    public void UpdateTile(Vector2Int tilePos, Vector2Int newTilePos )
    {   
        BoardGrid[newTilePos.x, newTilePos.y] = BoardGrid[tilePos.x, tilePos.y];
        BoardGrid[tilePos.x, tilePos.y] = null;
    }
    
    private void SetBoard(TileSO[,] grid)
    {
        BoardGrid = grid;

        for (int x = 0; x < BoardGrid.GetLength(0); x++)
        {
            for (int y = 0; y < BoardGrid.GetLength(1); y++)
            {
                GameObject tile = Instantiate(tilePrefab, CalcTilePosition(x, y), Quaternion.identity);
                TileSO tileData = _tileCatalog.GetTile(BoardGrid[x, y].id);
                tile.GetComponent<Tile>().Initialize(tileData, x, y);
            }
        }
    }
    
    private static Vector3 CalcTilePosition(float x, float y)
    {
        // Make the spawn at top left instead of center
        // this way the area of the tile is (0-1, 0-1, 0) instead of (-0.5-0.5, -0.5-0.5, 0)
        return new Vector3(x, y, 0f) + new Vector3(0.5f, -0.5f, 0f);
    }
}