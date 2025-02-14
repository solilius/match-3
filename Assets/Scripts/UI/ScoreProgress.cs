using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreProgress : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private List<Star> stars;

    private int _scoreToPassLevel;

    public void Initialize(int scoreToPassLevel)
    {
        slider.value = 0;
        _scoreToPassLevel = scoreToPassLevel;
        slider.maxValue = _scoreToPassLevel;
        stars.ForEach(star => star.ResetStar());
    }

    public IEnumerator UpdateScore(int score)
    {
        while (slider.value < score)
        {
            slider.value += 1;
            HandleStars(score);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void HandleStars(int score)
    {
        for (int i = 0; i < stars.Count; i++)
        {
            if (!stars[i].IsGained && score >= (_scoreToPassLevel / stars.Count) * (i + 1))
            {
                stars[i].GainStar();
            }
        }
    }
}