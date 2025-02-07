using UnityEngine;
using UnityEngine.PlayerLoop;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float dragDistance = 5f;

    private TileSO[,] boardGrid;
    private TilesCatalog tileCatalog;

    private bool _isDragging;
    private Vector3 _startPosition;

    public void Initialize(TilesCatalog catalog, TileSO[,] grid)
    {
        tileCatalog = catalog;
        SetBoard(grid);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = GetMouseWorldPosition();

        if (Input.GetMouseButtonDown(0) && !_isDragging)
        {
            _isDragging = true;
            _startPosition = currentPosition;
        }
        
        if (_isDragging && IsValidDrag(_startPosition, currentPosition))
        {
            Vector3 direction = GetDirection(_startPosition, currentPosition);
            TileSO firstTile = boardGrid[Mathf.FloorToInt(_startPosition.x), Mathf.FloorToInt(_startPosition.y)];
            TileSO secondTile = boardGrid[Mathf.FloorToInt(_startPosition.x) + (int)direction.x, Mathf.FloorToInt(_startPosition.y) + (int)direction.y];
            Debug.Log($"{firstTile.id} -> {secondTile.id}");
            _isDragging = false;
        }
    }

    private static Vector3 GetDirection(Vector3 from, Vector3 to)
    {
        Vector3 diff = to - from;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            return diff.x > 0 ? Vector3.right : Vector3.left;
        }

        return diff.y > 0 ? Vector3.up : Vector3.down;
    }

    private bool IsWithinGrid(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);
        return x >= 0 && x < boardGrid.GetLength(0) && y >= 0 && y < boardGrid.GetLength(1);
    }

    private bool IsValidDrag(Vector3 startPosition, Vector3 endPosition)
    {
        if (startPosition == endPosition) return false;
        if (!IsWithinGrid(startPosition) || !IsWithinGrid(endPosition)) return false;

        if (Vector3.Distance(_startPosition, Input.mousePosition) < dragDistance) return false;

        return true;
    }

    private void SetBoard(TileSO[,] grid)
    {
        boardGrid = grid;

        for (int x = 0; x < boardGrid.GetLength(0); x++)
        {
            for (int y = 0; y < boardGrid.GetLength(1); y++)
            {
                GameObject tile = Instantiate(tilePrefab, CalcTilePosition(x, y), Quaternion.identity);
                TileSO tileData = tileCatalog.GetTile(boardGrid[x, y].id);
                tile.GetComponent<Tile>().Initialize(tileData, x, y);
            }
        }
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 screenPosition = Input.mousePosition;
        screenPosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(screenPosition);
    }

    private Vector3 CalcTilePosition(int x, int y)
    {
        // Make the spawn at top left instead of center
        // this way the area of the tile is (0-1, 0-1, 0) instead of (-0.5-0.5, -0.5-0.5, 0)
        return new Vector3(x, -y, 0f) + new Vector3(0.5f, -0.5f, 0f); 
    }
}