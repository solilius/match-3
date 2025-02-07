using UnityEngine;
using UnityEngine.PlayerLoop;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float dragDistance = .5f;

    private TileSO[,] boardGrid;
    private TilesCatalog tileCatalog;

    private bool _isDragging;
    private Vector2 _startDragPosition;
    private Vector2Int _selectedTilePosition;

    public void Initialize(TilesCatalog catalog, TileSO[,] grid)
    {
        tileCatalog = catalog;
        SetBoard(grid);
    }

    void Update()
    {
        Vector2 currentPosition = GetMouseWorldPosition();
        SetSelectedTile(currentPosition);

        if (Input.GetMouseButtonUp(0)) _isDragging = false;
        if (!IsValidDrag(_startDragPosition, currentPosition)) return;

        Vector2 direction = GetDirection(_startDragPosition, currentPosition);

        Vector2 draggedPosition = new Vector2(_selectedTilePosition.x + direction.x, _selectedTilePosition.y + direction.y);
        if (IsWithinGrid(draggedPosition, out Vector2Int draggedGridPosition))
        {
            TileSO firstTile = boardGrid[_selectedTilePosition.x, _selectedTilePosition.y];
            TileSO secondTile = boardGrid[draggedGridPosition.x, draggedGridPosition.y];
            Debug.Log($"{firstTile.id} -> {secondTile.id}");
        }
        else
        {
            Debug.Log($" Out: {_selectedTilePosition} -> {draggedGridPosition}");
        }
        
        _isDragging = false;
    }

    private void SetSelectedTile(Vector2 currentPosition)
    {
        if (!_isDragging && Input.GetMouseButtonDown(0))
        {
            _isDragging = true;

            if (IsWithinGrid(currentPosition, out Vector2Int gridPosition))
            {
                _startDragPosition = currentPosition;
                _selectedTilePosition = gridPosition;
            }
        }
    }

    private static Vector2 GetDirection(Vector2 from, Vector2 to)
    {
        Vector2 diff = to - from;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            return diff.x > 0 ? Vector2.right : Vector2.left;
        }

        return diff.y > 0 ? Vector2.up : Vector2.down;
    }

    private bool IsWithinGrid(Vector2 position, out Vector2Int gridPosition)
    {
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);
        gridPosition = new Vector2Int(x, y);

        return x >= 0 && x < boardGrid.GetLength(0) && y >= 0 && y < boardGrid.GetLength(1);
    }
    
    private bool IsValidDrag(Vector2 startPosition, Vector2 endPosition)
    {
        return _isDragging && Vector2.Distance(startPosition, endPosition) >= dragDistance;
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

    private Vector2 GetMouseWorldPosition()
    {
        Vector2 worldPosition= Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(worldPosition.x, -worldPosition.y);
    }

    private Vector3 CalcTilePosition(int x, int y)
    {
        // Make the spawn at top left instead of center
        // this way the area of the tile is (0-1, 0-1, 0) instead of (-0.5-0.5, -0.5-0.5, 0)
        return new Vector3(x, -y, 0f) + new Vector3(0.5f, -0.5f, 0f);
    }
}