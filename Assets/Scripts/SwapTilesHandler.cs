using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwapTilesHandler : MonoBehaviour
{
    [SerializeField] private float dragDistance = .5f;
    [SerializeField] private float swapDuration = .5f;

    private Board _board;
    private MatchFinder _matchFinder;
    private MatchHandler _matchHandler;

    private bool _isDragging;
    private Vector2 _startDragPosition;
    private Vector2Int _selectedTilePos;

    void Start()
    {
        _board = GetComponent<Board>();
        _matchFinder = GetComponent<MatchFinder>();
        _matchHandler = GetComponent<MatchHandler>();
    }

    void Update()
    {
        HandleDragging();
    }

    private void HandleDragging()
    {
        if (Input.GetMouseButtonUp(0)) _isDragging = false;
        
        Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (!_isDragging && Input.GetMouseButtonDown(0) && IsDraggable(currentPosition, out Vector2Int gridPosition))
        {
            _isDragging = true;
            _startDragPosition = currentPosition;
            _selectedTilePos = gridPosition;
        }

        if (!_isDragging) return;
        if (IsValidDrag(_startDragPosition, currentPosition))
        {
            HandleSwap(currentPosition);
        }
    }

    private void HandleSwap(Vector2 currentPosition)
    {
        Vector2 direction = GetDirection(_startDragPosition, currentPosition);
        Vector2 draggedPosition = new Vector2(_selectedTilePos.x + direction.x, _selectedTilePos.y + direction.y);

        if (IsDraggable(draggedPosition, out Vector2Int draggedGridPosition))
        {
            StartCoroutine(SwapTiles(draggedGridPosition, direction));
        }

        _isDragging = false;
    }

    private IEnumerator SwapTiles(Vector2Int draggedGridPosition, Vector2 direction)
    {
        _board.SwitchTiles(_selectedTilePos, draggedGridPosition);

        TileEvents.UpdateTilePosition(this, _selectedTilePos, direction, swapDuration);
        TileEvents.UpdateTilePosition(this, draggedGridPosition, direction * -1, swapDuration);

        yield return new WaitForSeconds(swapDuration); // swap back without delay

        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();

        matches.UnionWith(_matchFinder.GetMatches(_board.BoardGrid[_selectedTilePos.x, _selectedTilePos.y]?.variant,
            _selectedTilePos));
        matches.UnionWith(_matchFinder.GetMatches(_board.BoardGrid[draggedGridPosition.x, draggedGridPosition.y]?.variant,
            draggedGridPosition));

        if (matches.Count == 0)
        {
            _board.SwitchTiles(draggedGridPosition, _selectedTilePos);
            TileEvents.UpdateTilePosition(this, _selectedTilePos, direction, swapDuration);
            TileEvents.UpdateTilePosition(this, draggedGridPosition, direction * -1, swapDuration);
        }
        else
        {
            _matchHandler.HandleMatches(matches.ToList());
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

    private bool IsDraggable(Vector2 position, out Vector2Int gridPosition)
    {
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.CeilToInt(position.y);
        gridPosition = new Vector2Int(x, y);

        return _board.IsInGrid(x, y) && _board.BoardGrid[gridPosition.x, gridPosition.y] != null;
    }
    
    private bool IsValidDrag(Vector2 startPosition, Vector2 endPosition)
    {
        return _isDragging && Vector2.Distance(startPosition, endPosition) >= dragDistance;
    }
}