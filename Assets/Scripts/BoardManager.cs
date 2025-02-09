using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private float fallDuration = 0.05f;
    [SerializeField] private GameObject tilePrefab;

    private TilesCatalog _tileCatalog;
    public Board Board { get; private set; }

    public void Initialize(TilesCatalog catalog, string[,] grid)
    {
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

    public void AddTile(Vector2Int newTile,  TileSO tileData)
    {
        GameObject tile = Instantiate(tilePrefab, CalcTilePosition(newTile), Quaternion.identity);
        tile.GetComponent<Tile>().Initialize(tileData, newTile.x, newTile.y);
        Board.BoardGrid[newTile.x, newTile.y] = new GameTile(tile.GetInstanceID(), tileData.variant);
    }
    
    public void DropTiles()
    {
        List<Vector2Int> holes = Board.GetHoles();
        var holesSet = new HashSet<Vector2Int>(holes);

        for (int x = 0; x < Board.BoardGrid.GetLength(0); x++)
        {
            int dropCount = 0;
            for (int y = 0; y < Board.BoardGrid.GetLength(1); y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (holesSet.Contains(pos))
                {
                    dropCount++;
                }
                else if (dropCount > 0)
                {
                    Vector2Int nextPos = pos + new Vector2Int(0, -dropCount);
                    UpdateTilePositions(pos, nextPos);
                }
            }
        }
    }

    public void FillNewTiles()
    {
        List<Vector2Int> holes = Board.GetHoles();
        int boardHeight = Board.BoardGrid.GetLength(1);

        foreach (Vector2Int hole in holes)
        {
            Vector2Int spawnPos = new Vector2Int(hole.x, boardHeight - 1);
            AddTile(spawnPos, _tileCatalog.GetTileVariant());
        
            UpdateTilePositions(spawnPos, hole);
        }
    }


    private void UpdateTilePositions(Vector2Int tilePos, Vector2Int newPos)
    {
        TileEvents.UpdateTilePosition(this, Board.GetTile(tilePos).GameObjectId, newPos, fallDuration);
        Board.UpdateTile(tilePos, newPos);
    }

    public static Vector3 CalcTilePosition(Vector2Int position)
    {
        // Make the spawn at top left instead of center
        // this way the area of the tile is (0-1, 0-1, 0) instead of (-0.5-0.5, -0.5-0.5, 0)
        return new Vector3(position.x, position.y, 0f) + new Vector3(0.5f, -0.5f, 0f);
    }
}