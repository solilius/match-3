using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MatchHandler : MonoBehaviour
{
    [SerializeField] private int minimumMatch = 3;

    private BoardManager _boardManager;
    private ScoreManager _scoreManager;

    public readonly Vector2Int[] SearchVectors =
    {
        new(1, 0), // Horizontal
        new(0, 1), // Vertical
    };

    void Awake()
    {
        _boardManager = GetComponent<BoardManager>();
    }

    void Start()
    {
        _scoreManager = FindFirstObjectByType<ScoreManager>(); // get from params
    }

    public void HandleMatches(HashSet<Vector2Int> matches)
    {
        List<Vector2Int> matchQueue = new List<Vector2Int>(matches);

        int counter = 0;
        while (counter < matchQueue.Count)
        {
            Vector2Int match = matchQueue[counter];
            counter++;
            TileSO tile = _boardManager.Board.GetTile(match)?.Data;

            switch (tile?.tileType)
            {
                case TileType.Fruit:
                    _scoreManager.AddScore(((FruitSO)tile).score);
                    break;
                case TileType.Power:
                    HashSet<Vector2Int> newMatches = GetPowerMatches(match);
                    matchQueue.AddRange(newMatches);
                    matchQueue = matchQueue.Distinct().ToList();
                    break;
                default:
                    throw new Exception($"Invalid tile type: {tile?.tileType}");
            }
        }

        matchQueue.ToList().ForEach(_boardManager.RemoveTile);
        StartCoroutine(_boardManager.UpdateBoard());
    }

    public void HandlePossibleMatches(HashSet<Vector2Int> changes)
    {
        if (changes.Count == 0) return;

        HashSet<Vector2Int> matches = GetMatches(changes);

        if (matches.Count > 0)
        {
            HandleMatches(matches);
        }
    }

    public HashSet<Vector2Int> GetMatches(HashSet<Vector2Int> changes)
    {
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();

        foreach (Vector2Int change in changes)
        {
            TileSO tile = _boardManager.Board.GetTile(change)?.Data;

            matches.UnionWith(GetTileMatches(tile?.variant, change));
        }

        return matches;
    }

    public HashSet<Vector2Int> GetPowerMatches(Vector2Int position)
    {
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();
        TileSO tileData = _boardManager.Board.GetTile(position)?.Data;

        if (tileData?.tileType == TileType.Power)
        {
            matches = GetPowerUpMatch((PowerSO)tileData, position);
        }

        return matches;
    }

    public List<Vector2Int> GetTileMatches(string tileVariant, Vector2Int tilePosition)
    {
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();

        foreach (Vector2Int vector in SearchVectors)
        {
            List<Vector2Int> currentMatches = new List<Vector2Int> { tilePosition };

            bool isForwardVectorRunning = true;
            bool isBackwardVectorRunning = true;
            int index = 1;

            while (index < Math.Min(_boardManager.Board.BoardHeight, _boardManager.Board.BoardWidth)
                   && (isForwardVectorRunning || isBackwardVectorRunning))
            {
                Vector2Int vectorModifier = vector * index;
                Vector2Int checkPositionA = tilePosition + vectorModifier;
                Vector2Int checkPositionB = tilePosition + vectorModifier * -1;

                if (isForwardVectorRunning && IsMatchingTile(checkPositionA, tileVariant))
                    currentMatches.Add(checkPositionA);
                else isForwardVectorRunning = false;

                if (isBackwardVectorRunning && IsMatchingTile(checkPositionB, tileVariant))
                    currentMatches.Add(checkPositionB);
                else isBackwardVectorRunning = false;

                if (isForwardVectorRunning || isBackwardVectorRunning) index++;
                else break;
            }

            if (currentMatches.Count >= minimumMatch) matches.AddRange(currentMatches);
        }

        return matches.OrderBy(v => v.y).ThenBy(v => v.x).ToList();
    }

    private HashSet<Vector2Int> GetPowerUpMatch(PowerSO powerUp, Vector2Int position)
    {
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>() { position };

        foreach (Vector2Int vector in powerUp.popDirections)
        {
            for (int x = 1; x <= powerUp.popRadius; x++)
            {
                for (int y = 1; y <= powerUp.popRadius; y++)
                {
                    Vector2Int checkPosition = position + vector * new Vector2Int(x, y);
                    if (_boardManager.Board.GetTile(checkPosition) != null)
                    {
                        matches.Add(checkPosition);
                    }
                }
            }
        }

        return matches;
    }

    private bool IsMatchingTile(Vector2Int checkPosition, string tileVariant)
    {
        return tileVariant != null && _boardManager.Board.GetTile(checkPosition)?.Data.variant == tileVariant;
    }
}