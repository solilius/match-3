using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GenLogic
{
    Random,

    // Special,
    Match2,
    Match3,
}

public class TileGenerator : MonoBehaviour
{
    private TilesCatalog _tileCatalog;
    private GameTile[,] _board;

    private Dictionary<GenLogic, float> _tileGenMap;
    private Dictionary<GenLogic, Func<Vector2Int, TileSO>> _genLogicFunctions;

    public void Initialize(TilesCatalog catalog, Dictionary<GenLogic, float> tileGenMap)
    {
        _tileCatalog = catalog;
        _tileGenMap = tileGenMap;
        _genLogicFunctions = new Dictionary<GenLogic, Func<Vector2Int, TileSO>>
        {
            { GenLogic.Random, GetRandomTile },
            // { GenLogic.Special, GetRandomTile },
            { GenLogic.Match2, GetMatch2Tile },
            { GenLogic.Match3, GetMatch3Tile },
        };
    }

    public TileSO[,] GenerateBoard(int width, int height, Dictionary<GenLogic, float> boardGenMap)
    {
        TileSO[,] grid = new TileSO[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = GenerateTile(new Vector2Int(x, y), boardGenMap);
            }
        }

        return grid;
    }

    public TileSO GenerateTile(Vector2Int position, Dictionary<GenLogic, float> genMap = null)
    {
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        Dictionary<GenLogic, float> sortedDict = genMap ?? _tileGenMap
            .OrderByDescending(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);

        foreach (var genLogic in sortedDict)
        {
            if (genLogic.Value >= randomValue)
            {
                return _genLogicFunctions[genLogic.Key](position);
            }
        }

        return GetRandomTile(position);
    }

    private TileSO GetRandomTile(Vector2Int position)
    {
        return _tileCatalog.GetRandomTile();
    }

    private TileSO GetMatch2Tile(Vector2Int position)
    {
        return GetRandomTile(position);
    }

    private TileSO GetMatch3Tile(Vector2Int position)
    {
        return GetRandomTile(position);
    }
}