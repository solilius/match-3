using UnityEngine;

public enum TileType
{
    Fruit,
    Power,
}

public abstract class TileSO : ScriptableObject
{
    public TileType tileType;
    public string id;
    public Sprite sprite;
}