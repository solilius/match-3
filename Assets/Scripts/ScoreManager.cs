using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _score = 0;

    public void AddScore(int scoreToAdd)
    {
        _score += scoreToAdd;
    }
}
