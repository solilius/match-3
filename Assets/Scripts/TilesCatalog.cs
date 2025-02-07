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

    public TileSO GetTile(string tileId)
    {
        return _tiles.Find(tile => tile.id == tileId);
    }
}