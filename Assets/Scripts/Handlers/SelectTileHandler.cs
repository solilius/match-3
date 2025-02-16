using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectTileHandler : MonoBehaviour
{
    public static event EventHandler OnPowerUsed;

    [SerializeField] private float longPressThreshold = 0.5f;
    [SerializeField] private float pressedScale = 1.1f;

    private BoardManager _boardManager;
    private MatchHandler _matchHandler;

    private float? _pressStartTime;
    private Vector2Int? _pressedTile;

    void Awake()
    {
        _boardManager = GetComponent<BoardManager>();
        _matchHandler = GetComponent<MatchHandler>();
    }

    void Update()
    {
        HandleSelect();
    }

    private void HandleSelect()
    {
        Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonUp(0) && _pressStartTime != null && _pressedTile != null)
        {
            HandleCanceledLongPress(_pressedTile.Value);
        }

        if (Input.GetMouseButtonDown(0) && _pressStartTime == null &&
            IsSelectable(currentPosition, out Vector2Int gridPosition))
        {
            HandleLongPress(gridPosition);
        }

        if (_pressStartTime != null && _pressedTile != null && Time.time - _pressStartTime > longPressThreshold)
        {
            HandlePowerTriggered(_pressedTile.Value);
        }
    }

    private void HandlePowerTriggered(Vector2Int pressedTile)
    {
        OnPowerUsed?.Invoke(this, EventArgs.Empty);
        HandleCanceledLongPress(pressedTile);
        HashSet<Vector2Int> matches = _matchHandler.GetPowerMatches(pressedTile);
        if (matches.Count > 0) _matchHandler.HandleMatches(matches);
    }

    private void HandleLongPress(Vector2Int gridPosition)
    {
        _pressStartTime = Time.time;
        _pressedTile = gridPosition;
        int gameObjectId = _boardManager.Board.GetTile(_pressedTile.Value).GameObjectId;
        TileEvents.PowerTilePressed(this, gameObjectId, pressedScale, longPressThreshold);
    }

    private void HandleCanceledLongPress(Vector2Int pressedTile)
    {
        TileEvents.PowerTilePressed(this, _boardManager.Board.GetTile(pressedTile).GameObjectId, 1, 0);
        _pressStartTime = null;
        _pressedTile = null;
    }

    private bool IsSelectable(Vector2 position, out Vector2Int gridPosition)
    {
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.CeilToInt(position.y);
        gridPosition = new Vector2Int(x, y);
        GameTile tile = _boardManager.Board.GetTile(gridPosition);
        return tile?.Data.tileType == TileType.Power;
    }
}