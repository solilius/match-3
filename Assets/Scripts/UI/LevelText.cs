using TMPro;
using UnityEngine;

public class LevelText : MonoBehaviour
{
    private TMP_Text _levelText;
    
    void Awake()
    {
        _levelText = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        GameManager.OnLevelUpdated += UpdateText;
    }
    
    void OnDisable()
    {
        GameManager.OnLevelUpdated -= UpdateText;
    }
    
    private void UpdateText(object that, int level)
    {
        _levelText.text = $"Level: {level}";
    }
}
