using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ScoreManager : MonoBehaviour
{ 
    public static event EventHandler OnLevelCleared;
    public int levelMaxScore;
    public int score = 0;
    
    [SerializeField] private ScoreProgress scoreProgress;
    [SerializeField] private ScoreText scoreText;
    [SerializeField] private TMP_Text maxScoreText;
        
    public void StartLevel(int maxScore)
    {
        score = 0;
        levelMaxScore = maxScore;
        maxScoreText.text = $"Max: {levelMaxScore}";
        scoreProgress.Initialize(levelMaxScore);
        scoreText.UpdateTextScore(score);
    }
    
    public bool IsLevelCleared()
    {
        return score >= levelMaxScore;
    }
    
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.UpdateTextScore(score);
        scoreProgress.UpdateScore(score);

        if (score >= levelMaxScore)
        {
            OnLevelCleared?.Invoke(this, EventArgs.Empty);
        }
    }
}