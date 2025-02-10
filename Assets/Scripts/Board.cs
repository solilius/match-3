using System.Collections.Generic;
using UnityEngine;

public class GameTile
{
    public int GameObjectId { get; }
    public string Variant { get; }

    public GameTile(int gameObjectId, string variant)
    {
        GameObjectId = gameObjectId;
        Variant = variant;
    }
}

public class Board
{
    public GameTile[,] BoardGrid { get; private set; }
    public int BoardWidth => BoardGrid.GetLength(0);
    public int BoardHeight => BoardGrid.GetLength(1) - 1;
    public int BoardHeightWithSpawner => BoardGrid.GetLength(1);
    public int SpawnerRow => BoardGrid.GetLength(1) - 1;

    public Board(int width, int height)
    {
        BoardGrid = new GameTile[width, height + 1];
    }

    public GameTile GetTile(Vector2Int pos, bool includeSpawner = false)
    {
        if (!IsInGrid(pos.x, pos.y, includeSpawner)) return null;
        return BoardGrid[pos.x, pos.y];
    }

    public bool IsInGrid(int x, int y, bool includeSpawner = false)
    {
        int boardHeight = includeSpawner ? BoardHeightWithSpawner : BoardHeight;
        return BoardGrid != null &&
               x >= 0 && x < BoardWidth &&
               y >= 0 && y < boardHeight;
    }

    public void SwitchTiles(Vector2Int tileA, Vector2Int tileB)
    {
        (BoardGrid[tileA.x, tileA.y], BoardGrid[tileB.x, tileB.y]) =
            (BoardGrid[tileB.x, tileB.y], BoardGrid[tileA.x, tileA.y]);
    }

    public void RemoveTile(Vector2Int tile)
    {
        BoardGrid[tile.x, tile.y] = null;
    }

    public void UpdateTile(Vector2Int tilePos, Vector2Int newTilePos)
    {
        BoardGrid[newTilePos.x, newTilePos.y] = BoardGrid[tilePos.x, tilePos.y];
        BoardGrid[tilePos.x, tilePos.y] = null;
    }

    public List<Vector2Int> GetLowestRowHoles()
    {
        List<Vector2Int> holes = new List<Vector2Int>();
        for (int y = 0; y < BoardHeight; y++)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                if (BoardGrid[x, y] == null) holes.Add(new Vector2Int(x, y));
            }

            if (holes.Count > 0) break;
            holes.Clear();
        }

        return holes;
    }

    private static Vector3 CalcTilePosition(float x, float y)
    {
        // Make the spawn at top left instead of center
        // this way the area of the tile is (0-1, 0-1, 0) instead of (-0.5-0.5, -0.5-0.5, 0)
        return new Vector3(x, y, 0f) + new Vector3(0.5f, -0.5f, 0f);
    }
}