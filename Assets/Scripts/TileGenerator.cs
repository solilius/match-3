using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GenLogic
{
    Random,
    RandomFruit,
    RandomPower,
    NoMatch,
    Match2,
    Match3,
}

public class TileGenerator : MonoBehaviour
{
    private TilesCatalog _tileCatalog;
    private MatchHandler _matchHandler;

    private BoardManager _boardManager;

    private List<ProcGenRule> _tileGenMap;
    private Dictionary<GenLogic, Func<Vector2Int, TileSO>> _genLogicFunctions;

    public void Initialize(TilesCatalog catalog, List<ProcGenRule> tileGenMap)
    {
        _boardManager = GetComponent<BoardManager>();
        _matchHandler = GetComponent<MatchHandler>();

        _tileCatalog = catalog;
        _tileGenMap = tileGenMap;
        _genLogicFunctions = new Dictionary<GenLogic, Func<Vector2Int, TileSO>>
        {
            { GenLogic.Random, GetRandomTile },
            { GenLogic.RandomFruit, GetRandomFruitTile },
            { GenLogic.RandomPower, GetRandomPowerTile },
            { GenLogic.NoMatch, GetNoMatchTile },
            { GenLogic.Match2, GetMatch2Tile },
            { GenLogic.Match3, GetMatch3Tile },
        };
    }

    public TileSO GenerateTile(Vector2Int position, List<ProcGenRule> genMap = null)
    {
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        var cumulative = 0f;
        var sortedDict = (genMap ?? _tileGenMap).OrderBy(x => x.Probability);

        foreach (var genLogic in sortedDict)
        {
            cumulative += genLogic.Probability;
            if (randomValue <= cumulative)
            {
                return _genLogicFunctions[genLogic.GenLogic](position);
            }
        }

        Debug.Log($"Failed to generate tile from map position: {position} value: {randomValue}");
        return GetRandomFruitTile(position);
    }

    private TileSO GetRandomTile(Vector2Int position = new Vector2Int())
    {
        return _tileCatalog.GetRandomTile();
    }

    private TileSO GetRandomFruitTile(Vector2Int position = new Vector2Int())
    {
        return _tileCatalog.GetRandomTile(TileType.Fruit);
    }

    private TileSO GetRandomPowerTile(Vector2Int position = new Vector2Int())
    {
        return _tileCatalog.GetRandomTile(TileType.Power);
    }

    private TileSO GetNoMatchTile(Vector2Int position)
    {
        HashSet<string> tilesAround = GetFruitsAround(position);
        List<string> allVariants = _tileCatalog.GetFruitVariants();
        List<string> notAround = allVariants.Where(variant => !tilesAround.Contains(variant)).ToList();

        if (notAround.Count == 0) return GetRandomFruitTile();
        string variant = notAround.ElementAt(UnityEngine.Random.Range(0, notAround.Count));
        return _tileCatalog.GetTileVariant(variant);
    }

    private TileSO GetMatch2Tile(Vector2Int position)
    {
        HashSet<string> tilesAround = GetFruitsAround(position);
        HashSet<string> willMatch3 = new HashSet<string>();

        foreach (string variant in tilesAround)
        {
            if (_matchHandler.GetTileMatches(variant, position).Count >= 3)
            {
                willMatch3.Add(variant);
            }
        }

        List<string> match2 = tilesAround.Where(variant => !willMatch3.Contains(variant)).ToList();
        if (match2.Count == 0)
        {
            List<string> allVariants = _tileCatalog.GetFruitVariants();
            List<string> notAround = allVariants.Where(variant => !tilesAround.Contains(variant)).ToList();

            string variant = notAround.ElementAt(UnityEngine.Random.Range(0, notAround.Count));
            return _tileCatalog.GetTileVariant(variant);
        }

        string randomVariant = match2.ElementAt(UnityEngine.Random.Range(0, match2.Count));
        return _tileCatalog.GetTileVariant(randomVariant);
    }

    private TileSO GetMatch3Tile(Vector2Int position)
    {
        HashSet<string> tilesAround = GetFruitsAround(position);

        foreach (string variant in tilesAround)
        {
            _matchHandler.GetTileMatches(variant, position);
            if (_matchHandler.GetTileMatches(variant, position).Count >= 3)
            {
                return _tileCatalog.GetTileVariant(variant);
            }
        }

        if (tilesAround.Count == 0) return GetRandomFruitTile();
        string randomVariant = tilesAround.ElementAt(UnityEngine.Random.Range(0, tilesAround.Count));
        return _tileCatalog.GetTileVariant(randomVariant);
    }

    private HashSet<string> GetFruitsAround(Vector2Int position)
    {
        HashSet<string> tilesAround = new HashSet<string>();

        foreach (Vector2Int vector in _matchHandler.SearchVectors)
        {
            GameTile tile = _boardManager.Board.GetTile(new Vector2Int(position.x + vector.x, position.y + vector.y));
            if (tile != null) tilesAround.Add(tile.Data.variant);

            tile = _boardManager.Board.GetTile(new Vector2Int(position.x - vector.x, position.y - vector.y));
            if (tile != null && tile.Data.tileType == TileType.Fruit) tilesAround.Add(tile.Data.variant);
        }

        return tilesAround;
    }
}