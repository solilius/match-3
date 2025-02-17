using System;

public partial class GameManager
{
    public static event EventHandler<int> OnMoveUsed;
    public static event EventHandler<int> OnLevelUpdated;

    void OnEnable()
    {
        SwapTilesHandler.OnSwapTiles += MoveUsed;
        SelectTileHandler.OnPowerUsed += MoveUsed;
        ScoreManager.OnLevelCleared += LevelCleared;
        PopupLevelFailed.OnLevelRestClicked += ResetLevel;
        PopupLevelCompleted.OnNextLevelClicked += NextLevel;
        PopupLevelCompleted.OnRetryClicked += ResetLevel;
    }

    void OnDisable()
    {
        SwapTilesHandler.OnSwapTiles -= MoveUsed;
        SelectTileHandler.OnPowerUsed -= MoveUsed;
        ScoreManager.OnLevelCleared -= LevelCleared;
        PopupLevelFailed.OnLevelRestClicked -= ResetLevel;
        PopupLevelCompleted.OnNextLevelClicked -= NextLevel;
        PopupLevelCompleted.OnRetryClicked -= ResetLevel;
    }

    private void MoveUsed(object sender, EventArgs e)
    {
        _moves--;
        bool isCleared = _scoreManager.IsLevelCleared();

        if (_moves > 0)
        {
            OnMoveUsed?.Invoke(this, _moves);
        }

        if (isCleared || _moves <= 0)
        {
            _popupManager.ShowPopup(isCleared ? PopupId.LevelComplete : PopupId.LevelFailed);
            _boardManager.SetLevelEnded();
        }
    }

    private void LevelCleared(object that, EventArgs e)
    {
        _boardManager.SetLevelEnded();
        _popupManager.ShowPopup(PopupId.LevelComplete);
    }

    private void ResetLevel(object that, EventArgs e)
    {
        StartLevel();
    }

    private void NextLevel(object that, EventArgs e)
    {
        LevelUp();
        StartLevel();
    }
}