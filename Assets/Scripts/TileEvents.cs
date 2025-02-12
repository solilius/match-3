using System;
using UnityEngine;

public class PowerTilePressedEventArgs : EventArgs
{
    public int GameObjectId { get; }
    public float ScaleTo { get; }
    public float Duration { get; }

    public PowerTilePressedEventArgs(int gameObjectId, float scaleTo, float duration)
    {
        GameObjectId = gameObjectId;
        ScaleTo = scaleTo;
        Duration = duration;
    }
}

public class UpdateTilePositionEventArgs : EventArgs
{
    public int GameObjectId { get; }
    public Vector2Int NewPosition { get; }
    public float Duration { get; }
    public bool IsSwap { get; }
    public bool IsSwapBack { get; }

    public UpdateTilePositionEventArgs(int gameObjectId, Vector2Int newPosition, float duration, bool isSwap,
        bool isSwapBack)
    {
        GameObjectId = gameObjectId;
        NewPosition = newPosition;
        Duration = duration;
        IsSwap = isSwap;
        IsSwapBack = isSwapBack;
    }
}

public static class TileEvents
{
    public static event EventHandler<UpdateTilePositionEventArgs> OnUpdateTilePosition;
    public static event EventHandler<PowerTilePressedEventArgs> OnPowerTilePressed;

    public static void UpdateTilePosition(object that, int gameObjectId, Vector2Int newPosition, float duration,
        bool isSwap = false, bool isSwapBack = false)
    {
        OnUpdateTilePosition?.Invoke(that,
            new UpdateTilePositionEventArgs(gameObjectId, newPosition, duration, isSwap, isSwapBack));
    }

    public static void PowerTilePressed(object that, int gameObjectId, float scaleTo, float duration)
    {
        OnPowerTilePressed?.Invoke(that, new PowerTilePressedEventArgs(gameObjectId, scaleTo, duration));
    }
}