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
        _scoreManager.AddScore(matches.Count * 10);
        matches.ToList().ForEach(_boardManager.RemoveTile);
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
            if (tile?.tileType != TileType.Fruit) continue;

            matches.UnionWith(GetTileMatches(tile?.variant, change));
        }

        return matches;
    }

    public HashSet<Vector2Int> GetPowerMatches(Vector2Int position)
    {
        string powerUpVariant = _boardManager.Board.GetTile(position)?.Data.variant;
        return new HashSet<Vector2Int>();
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

            while (isForwardVectorRunning || isBackwardVectorRunning)
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

    private bool IsMatchingTile(Vector2Int checkPosition, string tileVariant)
    {
        return _boardManager.Board.GetTile(checkPosition)?.Data.variant == tileVariant;
    }
}