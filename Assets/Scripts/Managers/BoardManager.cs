using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public class BoardManager : MonoBehaviour
{
    public static event EventHandler<MatchedTileEventArgs> OnMatched;
    public Board Board { get; private set; }

    [SerializeField] private float moveDuration = 0.05f;
    [SerializeField] private GameObject tilePrefab;

    private TileGenerator _tileGenerator;
    private ScoreManager _scoreManager;
    private MatchFinder _matchFinder;

    void Awake()
    {
        _matchFinder = GetComponent<MatchFinder>();
        _tileGenerator = GetComponent<TileGenerator>();
    }

    public void Initialize(int size, Dictionary<GenLogic, float> procGenBoard)
    {
        Board = new Board(size, size);
        _scoreManager = FindFirstObjectByType<ScoreManager>(); // get from params

        for (int y = 0; y < Board.BoardHeight; y++)
        {
            for (int x = 0; x < Board.BoardWidth; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                TileSO tile = _tileGenerator.GenerateTile(position, procGenBoard);
                AddTile(position, tile);
            }
        }
    }

    private void AddTile(Vector2Int newTile, TileSO tileData)
    {
        GameObject tile = Instantiate(tilePrefab, CalcTilePosition(newTile), Quaternion.identity, transform);
        tile.GetComponent<Tile>().Initialize(tileData, newTile.x, newTile.y);
        Board.BoardGrid[newTile.x, newTile.y] = new GameTile(tile.GetInstanceID(), tileData.tileType, tileData.variant);
    }
    
    private void RemoveTile(Vector2Int tilePosition)
    {
        int gameObjectId = Board.GetTile(tilePosition).GameObjectId;
        Board.RemoveTile(tilePosition);
        OnMatched?.Invoke(this, new MatchedTileEventArgs(gameObjectId, moveDuration));
    }

    public void HandleMatches(HashSet<Vector2Int> matches)
    {
        _scoreManager.AddScore(matches.Count * 10);
        matches.ToList().ForEach(RemoveTile);
        StartCoroutine(UpdateBoard());
    }

    private IEnumerator UpdateBoard()
    {
        HashSet<Vector2Int> changes = new HashSet<Vector2Int>();

        List<Vector2Int> holes = Board.GetLowestRowHoles();
        while (holes.Count > 0)
        {
            changes.UnionWith(holes);
            SpawnTiles(holes);
            holes.ForEach(DropTiles);
            holes = Board.GetLowestRowHoles();
            yield return new WaitForSeconds(moveDuration);
        }

        HashSet<Vector2Int> calculatedChangedPositions = CalculateChangedPositions(changes);
        HandlePossibleMatches(calculatedChangedPositions);
    }

    private HashSet<Vector2Int> CalculateChangedPositions(HashSet<Vector2Int> changes)
    {
        var lowestYPerX = new Dictionary<int, int>();
        foreach (var change in changes)
        {
            if (!lowestYPerX.TryGetValue(change.x, out int currentY) || change.y < currentY)
                lowestYPerX[change.x] = change.y;
        }

        var positions = new HashSet<Vector2Int>();
        foreach (var kvp in lowestYPerX)
        {
            for (int y = kvp.Value; y < Board.BoardHeight; y++)
                positions.Add(new Vector2Int(kvp.Key, y));
        }

        return positions;
    }

    private void HandlePossibleMatches(HashSet<Vector2Int> changes)
    {
        if (changes.Count == 0) return;

        HashSet<Vector2Int> matches = _matchFinder.GetMatches(changes);

        if (matches.Count > 0)
        {
            HandleMatches(matches);
        }
    }

    private void DropTiles(Vector2Int hole)
    {
        for (int y = hole.y + 1; y < Board.BoardHeightWithSpawner; y++)
        {
            Vector2Int tilePos = new Vector2Int(hole.x, y);
            if (Board.BoardGrid[tilePos.x, tilePos.y] != null)
            {
                UpdateTilePositions(tilePos, tilePos + Vector2Int.down);
            }
        }
    }

    private void SpawnTiles(List<Vector2Int> holes)
    {
        foreach (var hole in holes)
        {
            Vector2Int position = new Vector2Int(hole.x, Board.BoardHeight - GetNumOfTilesInColumn(hole.x));
            AddTile(new Vector2Int(hole.x, Board.SpawnerRow), _tileGenerator.GenerateTile(position));
        }
    }

    private void UpdateTilePositions(Vector2Int tilePos, Vector2Int newPos)
    {
        TileEvents.UpdateTilePosition(this, Board.GetTile(tilePos, true).GameObjectId, newPos, moveDuration);
        Board.UpdateTile(tilePos, newPos);
    }

    public static Vector3 CalcTilePosition(Vector2Int position)
    {
        // Make the spawn at top left instead of center
        // this way the area of the tile is (0-1, 0-1, 0) instead of (-0.5-0.5, -0.5-0.5, 0)
        return new Vector3(position.x, position.y, 0f) + new Vector3(0.5f, -0.5f, 0f);
    }

    private int GetNumOfTilesInColumn(int column)
    {
        int counter = 0;
        for (int y = 0; y < Board.BoardHeight; y++)
        {
            if (Board.BoardGrid[column, y] != null) // Check if the value is not null
            {
                counter++;
            }
        }

        return counter;
    }
}