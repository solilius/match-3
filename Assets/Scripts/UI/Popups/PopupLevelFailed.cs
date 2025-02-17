using System;
using UnityEngine;

public class PopupLevelFailed : MonoBehaviour, IPopup
{
    public static event EventHandler OnLevelRestClicked;
    
    public PopupId Id => PopupId.LevelFailed;
    public void DisplayPopup()
    {
        gameObject.SetActive(true);
    }

    public void QuitClicked()
    {
        HidePopup();
    }
    
    public void RetryClicked()
    {
        OnLevelRestClicked?.Invoke(this, EventArgs.Empty);
        HidePopup();
    }
    
    public void HidePopup()
    {
        gameObject.SetActive(false);
    }
}
