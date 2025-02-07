
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class TilesCatalog: MonoBehaviour
    {
        public List<TileSO> Tiles { get; private set; }

        void Awake()
        {
            Tiles = Resources.LoadAll<TileSO>("ScriptableObjects/Fruits").ToList();
        }

        public TileSO GetTile(string tileId)
        {
            return Tiles.Find(tile => tile.id == tileId);
        }
    }
