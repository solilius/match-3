using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    [SerializeField] private int minimumMatch = 3;

    private BoardManager _boardManager;

    private readonly Vector2Int[] _searchVectors =
    {
        new(1, 0), // Horizontal
        new(0, 1), // Vertical
    };

    void Awake()
    {
        _boardManager = GetComponent<BoardManager>();
    }

    public List<Vector2Int> GetMatches(HashSet<Vector2Int> changes)
    {
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();

        foreach (Vector2Int change in changes)
        {
            matches.UnionWith(GetTileMatches(_boardManager.Board.GetTile(change)?.Variant, change));
        }

        return matches.ToList();
    }
    
    public List<Vector2Int> GetTileMatches(string tileVariant, Vector2Int tilePosition)
    {
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();

        foreach (Vector2Int vector in _searchVectors)
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

                if (isForwardVectorRunning && IsMatchingTile(checkPositionA, tileVariant)) currentMatches.Add(checkPositionA);
                else isForwardVectorRunning = false;

                if (isBackwardVectorRunning && IsMatchingTile(checkPositionB, tileVariant)) currentMatches.Add(checkPositionB);
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
        return _boardManager.Board.GetTile(checkPosition)?.Variant == tileVariant;
    }
}