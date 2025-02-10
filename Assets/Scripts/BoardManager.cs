using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
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

    [SerializeField] private float moveDuration = 0.05f;
    [SerializeField] private GameObject tilePrefab;

    private TilesCatalog _tileCatalog;
    private ScoreManager _scoreManager;

    public Board Board { get; private set; }

    public void Initialize(TilesCatalog catalog, string[,] grid)
    {
        _scoreManager = FindFirstObjectByType<ScoreManager>(); // get from params
        Board = new Board(grid.GetLength(0), grid.GetLength(1));
        _tileCatalog = catalog;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                AddTile(new Vector2Int(x, y), _tileCatalog.GetTileVariant(grid[x, y]));
            }
        }
    }

    private void AddTile(Vector2Int newTile, TileSO tileData)
    {
        GameObject tile = Instantiate(tilePrefab, CalcTilePosition(newTile), Quaternion.identity);
        tile.GetComponent<Tile>().Initialize(tileData, newTile.x, newTile.y);
        Board.BoardGrid[newTile.x, newTile.y] = new GameTile(tile.GetInstanceID(), tileData.variant);
    }


    private void RemoveTile(Vector2Int tilePosition)
    {
        int gameObjectId = Board.GetTile(tilePosition).GameObjectId;
        Board.RemoveTile(tilePosition);
        OnMatched?.Invoke(this, new MatchedTileEventArgs(gameObjectId, moveDuration));
    }

    public void HandleMatches(List<Vector2Int> matches)
    {
        _scoreManager.AddScore(matches.Count * 10);
        matches.ForEach(RemoveTile);
        StartCoroutine(UpdateBoard());
        Debug.Log(Board.BoardGrid);
        // check new matches
    }

    private IEnumerator UpdateBoard()
    {
        List<Vector2Int> holes = Board.GetLowestRowHoles();
        while (holes.Count > 0)
        {
            SpawnTiles(holes);
            holes.ForEach(DropTiles);
            holes = Board.GetLowestRowHoles();
            yield return new WaitForSeconds(moveDuration);
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
            AddTile(new Vector2Int(hole.x, Board.SpawnerRow), _tileCatalog.GetTileVariant());
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
}