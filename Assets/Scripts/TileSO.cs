using UnityEngine;
using UnityEngine.Serialization;

public enum TileType
{
    Fruit,
    Power,
}

public abstract class TileSO : ScriptableObject
{
    public TileType tileType;
    public string variant;
    public Sprite sprite;
}