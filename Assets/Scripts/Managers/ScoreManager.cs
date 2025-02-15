using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ScoreManager : MonoBehaviour
{ 
    public static event EventHandler OnLevelCleared;
    public int levelMaxScore;
    public int score = 0;
    
    [SerializeField] private ScoreProgress scoreProgress;
    [SerializeField] private ScoreText scoreText;
    
    public void StartLevel(int maxScore)
    {
        score = 0;
        levelMaxScore = maxScore;
        scoreProgress.Initialize(levelMaxScore);
    }
    
    public bool IsLevelCleared()
    {
        return score >= levelMaxScore;
    }
    
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        StartCoroutine(scoreText.UpdateTextScore(score));
        StartCoroutine(scoreProgress.UpdateScore(score));

        if (score >= levelMaxScore)
        {
            OnLevelCleared?.Invoke(this, EventArgs.Empty);
        }
    }
}