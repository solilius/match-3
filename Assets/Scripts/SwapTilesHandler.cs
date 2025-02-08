using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPositionEventArgs : EventArgs
{
    public Vector2Int Position { get; }
    public Vector2Int Direction { get; }
    public float Duration { get; }

    public SwitchPositionEventArgs(Vector2Int position, Vector2 direction, float duration)
    {
        Position = position;
        Direction = Vector2Int.RoundToInt(direction);
        Duration = duration;
    }
}

public class SwapTilesHandler : MonoBehaviour
{
    public static event EventHandler<SwitchPositionEventArgs> OnSwitchPosition;

    [SerializeField] private float dragDistance = .5f;
    [SerializeField] private float swapDuration = .5f;

    private Board _board;
    private MatchHandler _matchHandler;

    private bool _isDragging;
    private Vector2 _startDragPosition;
    private Vector2Int _selectedTilePos;

    void Start()
    {
        _board = GetComponent<Board>();
        _matchHandler = GetComponent<MatchHandler>();
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
            _selectedTilePos = gridPosition;
        }

        if (IsValidDrag(_startDragPosition, currentPosition))
        {
            HandleSwap(currentPosition);
        }
    }

    private void HandleSwap(Vector2 currentPosition)
    {
        Vector2 direction = GetDirection(_startDragPosition, currentPosition);

        Vector2 draggedPosition =
            new Vector2(_selectedTilePos.x + direction.x, _selectedTilePos.y + direction.y);
        if (IsWithinGrid(draggedPosition, out Vector2Int draggedGridPosition))
        {
            StartCoroutine(SwapTiles(draggedGridPosition, direction));
        }

        _isDragging = false;
    }

    private IEnumerator SwapTiles(Vector2Int draggedGridPosition, Vector2 direction)
    {
        SwitchPositionEventArgs argsA = new SwitchPositionEventArgs(_selectedTilePos, direction, swapDuration);
        SwitchPositionEventArgs argsB = new SwitchPositionEventArgs(draggedGridPosition, direction * -1, swapDuration);
        _board.SwitchTiles(_selectedTilePos, draggedGridPosition);

        OnSwitchPosition?.Invoke(this, argsA);
        OnSwitchPosition?.Invoke(this, argsB);

        yield return new WaitForSeconds(swapDuration); // swap back without delay

        List<Vector2Int> matches = new List<Vector2Int>();

        matches.AddRange(_matchHandler.GetMatches(_board.BoardGrid[_selectedTilePos.x, _selectedTilePos.y].id,
            _selectedTilePos));
        matches.AddRange(_matchHandler.GetMatches(_board.BoardGrid[draggedGridPosition.x, draggedGridPosition.y].id,
            draggedGridPosition));

        if (matches.Count == 0)
        {
            _board.SwitchTiles(draggedGridPosition, _selectedTilePos);
            OnSwitchPosition?.Invoke(this, argsA);
            OnSwitchPosition?.Invoke(this, argsB);
        }
        else
        {
            _matchHandler.HandleMatches(matches);
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

        return _board.IsInGrid(x, y);
    }

    private bool IsValidDrag(Vector2 startPosition, Vector2 endPosition)
    {
        return _isDragging && Vector2.Distance(startPosition, endPosition) >= dragDistance;
    }

    private static Vector2 GetMouseWorldPosition()
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(worldPosition.x, -worldPosition.y);
    }
}