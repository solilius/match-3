using System;
using UnityEngine;

public class UpdateTilePositionEventArgs : EventArgs
{
    public int GameObjectId { get; }
    public Vector2Int NewPosition { get; }
    public float Duration { get; }

    public UpdateTilePositionEventArgs(int gameObjectId, Vector2Int newPosition, float duration)
    {
        GameObjectId = gameObjectId;
        NewPosition = newPosition;
        Duration = duration;
    }
}

public static class TileEvents
{
    public static event EventHandler<UpdateTilePositionEventArgs> OnUpdateTilePosition;
    
    public static void UpdateTilePosition(object that, int gameObjectId, Vector2Int newPosition, float duration)
    {
        OnUpdateTilePosition?.Invoke(that, new UpdateTilePositionEventArgs(gameObjectId, newPosition, duration));
    }
}

