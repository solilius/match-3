using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MatchedTileEventArgs : EventArgs
{
    public int GameObjectId { get; }
    public float Duration { get; }

    public MatchedTileEventArgs(int gameObjectId, float duration)
    {
        GameObjectId = gameObjectId;
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
        // get score

        foreach (Vector2Int match in matches)
        {
            StartCoroutine(HandleMatch(match));
        }

        DropTiles();
        // check new matches
    }

    private IEnumerator HandleMatch(Vector2Int match)
    {
        int gameObjectId = _board.GetTile(match).GameObjectId;
        _board.RemoveTile(match);
        OnMatched?.Invoke(this, new MatchedTileEventArgs(gameObjectId, removeTileDuration));
        yield return new WaitForSeconds(removeTileDuration);
    }

    private void DropTiles()
    {
        List<Vector2Int> holes = _board.GetHoles();
        var holesSet = new HashSet<Vector2Int>(holes);

        for (int x = 0; x < _board.BoardGrid.GetLength(0); x++)
        {
            int dropCount = 0;
            for (int y = 0; y < _board.BoardGrid.GetLength(1); y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (holesSet.Contains(pos))
                {
                    dropCount++;
                }
                else if (dropCount > 0)
                {
                    Vector2Int nextPos = pos + new Vector2Int(0, -dropCount);
                    StartCoroutine(UpdateTilePositions(pos, nextPos));
                }
            }
        }
    }


    private IEnumerator UpdateTilePositions(Vector2Int tilePos, Vector2Int newPos)
    {
        TileEvents.UpdateTilePosition(this, _board.GetTile(tilePos).GameObjectId, newPos, fallDuration);
        yield return new WaitForSeconds(fallDuration);
        _board.UpdateTile(tilePos, newPos);
    }
}

// ineed function to sync data  to ui or do the update