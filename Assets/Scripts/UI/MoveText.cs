using TMPro;
using UnityEngine;

public class MoveText : MonoBehaviour
{
    private TMP_Text _movesLeftText;
    
    void Awake()
    {
        _movesLeftText = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        GameManager.OnMoveUsed += UpdateText;
    }
    
    void OnDisable()
    {
        GameManager.OnMoveUsed -= UpdateText;
    }
    
    private void UpdateText(object that, int movesLeft)
    {
        _movesLeftText.color = movesLeft > 5 ? Color.white : Color.red;
        _movesLeftText.text = movesLeft.ToString();
    }
}
