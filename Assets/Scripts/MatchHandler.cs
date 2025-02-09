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

    private BoardManager _boardManager;
    private ScoreManager _scoreManager;

    void Awake()
    {
        _boardManager = GetComponent<BoardManager>();
        _scoreManager = FindFirstObjectByType<ScoreManager>();
    }

    public void HandleMatches(List<Vector2Int> matches)
    {
        _scoreManager.AddScore(matches.Count * 10);
        
        foreach (Vector2Int match in matches)
        {
            StartCoroutine(HandleMatch(match));
        }

        _boardManager.DropTiles();
        _boardManager.FillNewTiles();
        // check new matches
    }

    private IEnumerator HandleMatch(Vector2Int match)
    {
        int gameObjectId = _boardManager.Board.GetTile(match).GameObjectId;
        _boardManager.Board.RemoveTile(match);
        OnMatched?.Invoke(this, new MatchedTileEventArgs(gameObjectId, removeTileDuration));
        yield return new WaitForSeconds(removeTileDuration);
    }
    
}

// ineed function to sync data  to ui or do the update