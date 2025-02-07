using System;
using UnityEngine;

public class SwitchPositionEventArgs : EventArgs
{
    public Vector2Int Position { get; }
    public Vector2Int Direction { get; }

    public SwitchPositionEventArgs(Vector2Int position, Vector2 direction)
    {
        Position = position;
        Direction = Vector2Int.RoundToInt(direction);
    }
}

public class SwapTiles : MonoBehaviour
{
    public static event EventHandler<SwitchPositionEventArgs> OnSwitchPosition;

    [SerializeField] private float dragDistance = .5f;

    private Board board;

    private bool _isDragging;
    private Vector2 _startDragPosition;
    private Vector2Int _selectedTilePosition;

    void Awake()
    {
        board = GetComponent<Board>();
    }

    void Update()
    {
        HandleDragging();
    }

    private void HandleDragging()
    {
        if (Input.GetMouseButtonUp(0)) _isDragging = false;

        Vector2 currentPosition = GetMouseWorldPosition();

        if (!_isDragging && Input.GetMouseButtonDown(0) && IsWithinGrid(currentPosition, out Vector2Int gridPosition))
        {
            _isDragging = true;
            _startDragPosition = currentPosition;
            _selectedTilePosition = gridPosition;
        }

        if (IsValidDrag(_startDragPosition, currentPosition))
        {
            HandleSwap(currentPosition);
        }

        ;
    }

    private void HandleSwap(Vector2 currentPosition)
    {
        Vector2 direction = GetDirection(_startDragPosition, currentPosition);

        Vector2 draggedPosition =
            new Vector2(_selectedTilePosition.x + direction.x, _selectedTilePosition.y + direction.y);
        if (IsWithinGrid(draggedPosition, out Vector2Int draggedGridPosition))
        {
            OnSwitchPosition?.Invoke(this, new SwitchPositionEventArgs(_selectedTilePosition, direction));
            OnSwitchPosition?.Invoke(this, new SwitchPositionEventArgs(draggedGridPosition, direction * -1));
        }

        _isDragging = false;
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

        return x >= 0 && x < board.BoardGrid.GetLength(0) && y >= 0 && y < board.BoardGrid.GetLength(1);
    }

    private bool IsValidDrag(Vector2 startPosition, Vector2 endPosition)
    {
        return _isDragging && Vector2.Distance(startPosition, endPosition) >= dragDistance;
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(worldPosition.x, -worldPosition.y);
    }
}