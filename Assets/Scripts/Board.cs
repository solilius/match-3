using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameTile
{
    public int GameObjectId { get; }
    public string Variant { get; }

    public GameTile(int gameObjectId, string variant)
    {
        GameObjectId = gameObjectId;
        Variant = variant;
    }
}

public class Board : MonoBehaviour
{
    public GameTile[,] BoardGrid { get; private set; }

    [SerializeField] private GameObject tilePrefab;

    private TilesCatalog _tileCatalog;
    
    public void Initialize(TilesCatalog catalog, string[,] grid)
    {
        _tileCatalog = catalog;
        SetBoard(grid);
    }

    public GameTile GetTile(Vector2Int pos)
    {
        if (!IsInGrid(pos.x, pos.y)) return null;
        return BoardGrid[pos.x, pos.y];
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

    public void UpdateTile(Vector2Int tilePos, Vector2Int newTilePos)
    {
        BoardGrid[newTilePos.x, newTilePos.y] = BoardGrid[tilePos.x, tilePos.y];
        BoardGrid[tilePos.x, tilePos.y] = null;
    }

    public List<Vector2Int> GetHoles()
    {
        List<Vector2Int> holes = new List<Vector2Int>();
        for (int x = 0; x < BoardGrid.GetLength(0); x++)
        {
            for (int y = 0; y < BoardGrid.GetLength(1); y++)
            {
               if (BoardGrid[x, y] == null) holes.Add(new Vector2Int(x, y));
            }
        }
        return holes;
    }
    
    private void SetBoard(string[,] grid)
    {
        BoardGrid = new GameTile[grid.GetLength(0), grid.GetLength(1)];
        
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                GameObject tile = Instantiate(tilePrefab, CalcTilePosition(x, y), Quaternion.identity);
                TileSO tileData = _tileCatalog.GetTile(grid[x, y]);
                tile.GetComponent<Tile>().Initialize(tileData, x, y);
                BoardGrid[x, y] = new GameTile(tile.GetInstanceID(), tileData.variant);
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