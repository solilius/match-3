using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilesCatalog : MonoBehaviour
{
    private List<TileSO> _tiles;

    void Awake()
    {
        _tiles = Resources.LoadAll<TileSO>("ScriptableObjects/Fruits").ToList();
    }

    public TileSO GetTileVariant(string variant = null)
    {
        if (variant == null) return _tiles[Random.Range(0, _tiles.Count)];
        return _tiles.Find(tile => tile.variant == variant);
    }
}