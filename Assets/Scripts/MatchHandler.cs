using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MatchedTileEventArgs : EventArgs
{
    public Vector2Int Position { get; }
    public float Duration { get; }

    public MatchedTileEventArgs(Vector2Int position, float duration)
    {
        Position = position;
        Duration = duration;
    }
}

public class MatchHandler : MonoBehaviour
{
    public static event EventHandler<MatchedTileEventArgs> OnMatched;

    [SerializeField] private float removeTileDuration = 0.2f;
    [SerializeField] private float fallDuration = 0.05f;

    private Board _board;
    private ScoreManager _scoreManager;

    void Awake()
    {
        _board = GetComponent<Board>();
        _scoreManager = FindFirstObjectByType<ScoreManager>();
    }

    public void HandleMatches(List<Vector2Int> matches)
    {
        matches.ForEach(m => _scoreManager.AddScore(_board.GetScore(m)));
        matches.ForEach(_board.RemoveTile);

        foreach (Vector2Int match in matches)
        {
            StartCoroutine(HandleMatch(match));
        }

        // check new matches
    }

    private IEnumerator HandleMatch(Vector2Int match)
    {
        OnMatched?.Invoke(this, new MatchedTileEventArgs(match, removeTileDuration));
        yield return new WaitForSeconds(removeTileDuration);
        
        DropTiles(match);
    }

    private List<Vector2Int> GetFallingTilesAbove(Vector2Int matchedPos)
    {
        List<Vector2Int> fallingTiles = new List<Vector2Int>();

        for (int i = matchedPos.y + 1; i < _board.BoardGrid.GetLength(1); i++)
        {
            if (_board.BoardGrid[matchedPos.x, i] != null)
                fallingTiles.Add(new Vector2Int(matchedPos.x, i));
        }

        return fallingTiles;
    }

    private void DropTiles(Vector2Int dropPos)
    {
        List<Vector2Int> fallingTiles = GetFallingTilesAbove(dropPos);

        int floor = dropPos.y;

        foreach (var tilePos in fallingTiles)
        {
            // find how much needed to get from its y to the floor-1
            int distance = Mathf.Abs(floor - tilePos.y);
            floor++;
            StartCoroutine(UpdateTilePositions(tilePos, distance));
        }
    }

    private IEnumerator UpdateTilePositions(Vector2Int tilePos, int distance)
    {
        Vector2Int vector = Vector2Int.down * distance;
        Vector2Int newPos = tilePos + vector;
        TileEvents.UpdateTilePosition(this, tilePos, vector, fallDuration);
        yield return new WaitForSeconds(fallDuration);
        _board.UpdateTile(tilePos, newPos);
    }
}

// ineed function to sync data  to ui or do the update