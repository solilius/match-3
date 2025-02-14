using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private ScoreProgress scoreProgress;
    [SerializeField] private ScoreText scoreText;

    private int _scoreToPassLevel;
    private int _score = 0;

    public void StartLevel(int scoreToPassLevel)
    {
        _score = 0;
        _scoreToPassLevel = scoreToPassLevel;
        scoreProgress.Initialize(scoreToPassLevel);
    }

    public void AddScore(int scoreToAdd)
    {
        _score += scoreToAdd;
        StartCoroutine(scoreText.UpdateTextScore(_score));
        StartCoroutine(scoreProgress.UpdateScore(_score));

        if (_score >= _scoreToPassLevel)
        {
        }
    }
}