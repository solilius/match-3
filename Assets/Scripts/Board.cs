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

    public Board(int width, int height)
    {
        BoardGrid = new GameTile[width, height];
    }

    public GameTile GetTile(Vector2Int pos)
    {
        if (!IsInGrid(pos.x, pos.y)) return null;
        return BoardGrid[pos.x, pos.y];
    }

    public bool IsInGrid(int x, int y)
    {
        return BoardGrid != null &&
               x >= 0 && x < BoardGrid.GetLength(0) &&
               y >= 0 && y < BoardGrid.GetLength(1);
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

    public List<Vector2Int> GetHoles()
    {
        List<Vector2Int> holes = new List<Vector2Int>();
        for (int x = 0; x < BoardGrid.GetLength(0); x++)
        {
            for (int y = 0; y < BoardGrid.GetLength(1); y++)
            {
                if (BoardGrid[x, y] == null) holes.Add(new Vector2Int(x, y));
            }
        }

        return holes;
    }

    public List<Vector2Int> GetHoles(int col)
    {
        List<Vector2Int> holes = new List<Vector2Int>();
        for (int x = 0; x < BoardGrid.GetLength(0); x++)
        {
            if (BoardGrid[x, col] == null) holes.Add(new Vector2Int(x, col));
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