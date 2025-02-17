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


    public void UpdateTextScore(int newScore)
    {
        StartCoroutine(UpdateUI(newScore));
    }

    private IEnumerator UpdateUI(int newScore)
    {
        if (newScore == 0)
        {
            _score = newScore;
            _scoreText.text = $"Score: {_score}";
            yield return null;
        }

        while (_score < newScore)
        {
            _score += 1;
            _scoreText.text = $"Score: {_score}";
            yield return new WaitForSeconds(0.01f);
        }
    }
}