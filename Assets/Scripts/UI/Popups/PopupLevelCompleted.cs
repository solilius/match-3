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
        stars.ForEach(star => star.ResetStar());
        scoreText.text = $"Score: {_scoreManager.score}";

        StartCoroutine(UpdateStars());
    }

    public void RetryClicked()
    {
        Debug.Log($"RetryClicked");
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
        int scorePerStar = _scoreManager.levelMaxScore / stars.Count;
        int currentStarsValue = scorePerStar;

        for (int i = 0; i < stars.Count; i++)
        {
            if (_scoreManager.score >= currentStarsValue)
            {
                stars[i].GainStar();
            }

            currentStarsValue += scorePerStar;
            yield return new WaitForSeconds(.5f);
        }
    }
}