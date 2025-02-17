using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreProgress : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private List<Star> stars;

    private int _scoreToPassLevel;
    private Coroutine _updateUICoroutine;
    private int _score;

    public void Initialize(int scoreToPassLevel)
    {
        _score = 0;
        slider.value = _score;
        _scoreToPassLevel = scoreToPassLevel;
        slider.maxValue = _scoreToPassLevel;
        stars.ForEach(star => star.ResetStar());
    }

    public void UpdateScore(int score)
    {
        _score = score < _scoreToPassLevel ? score : _scoreToPassLevel;

        if (_updateUICoroutine != null)
        {
            StopCoroutine(_updateUICoroutine);
        }

        _updateUICoroutine = StartCoroutine(UpdateUI());
    }

    private IEnumerator UpdateUI()
    {
        while (slider.value < _score) 
        {
            slider.value += 2;
            HandleStars(Mathf.RoundToInt(slider.value));
            yield return new WaitForSeconds(0.001f);
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