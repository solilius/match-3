using System.Collections.Generic;
using UnityEngine;

public interface IPopup
{
    PopupId Id { get; }
    void DisplayPopup();
    void HidePopup();
}

public enum PopupId
{
    LevelFailed,
    LevelComplete,
    Settings,
}

public class PopupManager : MonoBehaviour
{
    private IPopup[] _popups;
    private IPopup _activePopup;

    void Awake()
    {
        _popups = transform.GetComponentsInChildren<IPopup>(true);
    }

    public void ShowPopup(PopupId id)
    {
        IPopup popup = _popups[(int)id];
        
        if (ShouldHideActivePopup(popup.Id)) _activePopup.HidePopup();
        
        _activePopup = popup;
        _activePopup?.DisplayPopup();
    }

    public void HidePopup()
    {
        if (_activePopup != null)
        {
            _activePopup.HidePopup();
            _activePopup = null;
        }
    }

    private bool ShouldHideActivePopup(PopupId id)
    {
        return _activePopup != null && id != PopupId.Settings;
    }
}