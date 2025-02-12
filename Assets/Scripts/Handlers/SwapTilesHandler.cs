using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapTilesHandler : MonoBehaviour
{
    [SerializeField] private float dragDistance = .5f;
    [SerializeField] private float swapDuration = .5f;

    private BoardManager _boardManager;
    private MatchFinder _matchFinder;

    private bool _isDragging;
    private Vector2 _startDragPosition;
    private Vector2Int _selectedTilePos;

    void Awake()
    {
        _boardManager = GetComponent<BoardManager>();
        _matchFinder = GetComponent<MatchFinder>();
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
        TileEvents.UpdateTilePosition(this, _boardManager.Board.GetTile(_selectedTilePos).GameObjectId,
            draggedGridPosition, swapDuration, true);
        TileEvents.UpdateTilePosition(this, _boardManager.Board.GetTile(draggedGridPosition).GameObjectId,
            _selectedTilePos, swapDuration, true);
        _boardManager.Board.SwitchTiles(_selectedTilePos, draggedGridPosition);

        yield return new WaitForSeconds(swapDuration); // swap back without delay

        HashSet<Vector2Int> matches = _matchFinder.GetMatches(new HashSet<Vector2Int>()
            { draggedGridPosition, _selectedTilePos });

        if (matches.Count == 0)
        {
            TileEvents.UpdateTilePosition(this, _boardManager.Board.GetTile(_selectedTilePos).GameObjectId,
                draggedGridPosition, swapDuration, true, true);
            TileEvents.UpdateTilePosition(this, _boardManager.Board.GetTile(draggedGridPosition).GameObjectId,
                _selectedTilePos, swapDuration, true, true);
            _boardManager.Board.SwitchTiles(draggedGridPosition, _selectedTilePos);
        }
        else
        {
            _boardManager.HandleMatches(matches);
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
        return _boardManager.Board.GetTile(gridPosition) != null;
    }

    private bool IsValidDrag(Vector2 startPosition, Vector2 endPosition)
    {
        return _isDragging && Vector2.Distance(startPosition, endPosition) >= dragDistance;
    }
}