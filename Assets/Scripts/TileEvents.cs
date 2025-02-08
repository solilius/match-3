using System;
using UnityEngine;

public class UpdateTilePositionEventArgs : EventArgs
{
    public Vector2Int Position { get; }
    public Vector2Int Direction { get; }
    public float Duration { get; }

    public UpdateTilePositionEventArgs(Vector2Int position, Vector2 direction, float duration)
    {
        Position = position;
        Direction = Vector2Int.RoundToInt(direction);
        Duration = duration;
    }
}

public static class TileEvents
{
    public static event EventHandler<UpdateTilePositionEventArgs> OnUpdateTilePosition;

    
    public static void UpdateTilePosition(object that, Vector2Int position, Vector2 direction, float duration)
    {
        OnUpdateTilePosition?.Invoke(that, new UpdateTilePositionEventArgs(position, direction, duration));
    }
}