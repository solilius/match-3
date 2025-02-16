using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupLevelCompleted : MonoBehaviour, IPopup
{
    public static event EventHandler OnNextLevelClicked;
    public static event EventHandler OnRetryClicked;

    [SerializeField] private List<Star> stars;
    [SerializeField] private TMP_Text scoreText;

    private ScoreManager _scoreManager;

    private void Awake()
    {
        _scoreManager = FindFirstObjectByType<ScoreManager>();
    }

    public PopupId Id => PopupId.LevelComplete;

    public void DisplayPopup()
    {
        gameObject.SetActive(true);
        scoreText.text = _scoreManager.score.ToString();
        StartCoroutine(UpdateStars());
    }

    public void RetryClicked()
    {
        OnRetryClicked?.Invoke(this, EventArgs.Empty);
        HidePopup();
    }

    public void NextClicked()
    {
        OnNextLevelClicked?.Invoke(this, EventArgs.Empty);
        HidePopup();
    }

    public void HidePopup()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator UpdateStars()
    {
        int score = 0;

        while (score <= _scoreManager.score)
        {
            for (int i = 0; i < stars.Count; i++)
            {
                if (!stars[i].IsGained && score >= (_scoreManager.levelMaxScore / stars.Count) * (i + 1))
                {
                    stars[i].GainStar();
                }
            }

            yield return new WaitForSeconds(.07f);
            score += 10;
        }
    }

    private void HandleStars(int score)
    {
        for (int i = 0; i < stars.Count; i++)
        {
            if (!stars[i].IsGained && score >= (_scoreManager.levelMaxScore / stars.Count) * (i + 1))
            {
                stars[i].GainStar();
            }
        }
    }
}