using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TilesCatalog : MonoBehaviour
{
    private List<TileSO> _tiles;
    private List<FruitSO> _fruitTiles;
    private List<PowerSO> _powerTiles;

    public void Initialize(List<TileSO> tiles)
    {
        _tiles = tiles;
        _fruitTiles = _tiles.Where(t => t.tileType == TileType.Fruit).Cast<FruitSO>().ToList();
        _powerTiles = _tiles.Where(t => t.tileType == TileType.Power).Cast<PowerSO>().ToList();
    }

    public TileSO GetTileVariant(string variant)
    {
        return _tiles.Find(tile => tile.variant == variant);
    }
    
    public TileSO GetRandomTile(TileType? type = null)
    {
        switch (type)
        {
            case TileType.Fruit:
                return _fruitTiles[Random.Range(0, _fruitTiles.Count)];
            case TileType.Power:
                return _powerTiles[Random.Range(0, _powerTiles.Count)];
            default:
                return _tiles[Random.Range(0, _tiles.Count)];
        }
    }

    public List<string> GetFruitVariants()
    {
        return _fruitTiles.Select(tile => tile.variant).ToList();
    }
}