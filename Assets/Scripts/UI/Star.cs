using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    [SerializeField] private Image markedStarImage;
    [SerializeField] private float scaleUpSpeed = .5f;
    [SerializeField] private float scaleUpSize = 1.5f;
    public bool IsGained { get; private set; }
    
    public void GainStar()
    {
        IsGained = true;
        markedStarImage.transform.DOScale(scaleUpSize, scaleUpSpeed).OnComplete(() =>
        {
            markedStarImage.transform.DOScale(1, scaleUpSpeed / 2);
        });
    }

    public void ResetStar()
    {
        IsGained = false;
        markedStarImage.transform.localScale = Vector3.zero;
    }
}