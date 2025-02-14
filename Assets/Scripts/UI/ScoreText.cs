using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    private TMP_Text _scoreText;
    private int _score;

    void Awake()
    {
        _score = 0;
        _scoreText = GetComponent<TMP_Text>();
    }

    public IEnumerator UpdateTextScore(int newScore)
    {
        if (newScore == 0)
        {
            _score = newScore;
            _scoreText.text = _score.ToString();
            yield return null;
        }

        while (_score < newScore)
        {
            _score += 1;
            _scoreText.text = _score.ToString();
            yield return new WaitForSeconds(0.01f);
        }
    }
}