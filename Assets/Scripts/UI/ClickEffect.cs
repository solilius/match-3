using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float scale = .97f;
    [SerializeField] private float duration = .1f;
    [SerializeField] private UnityEvent onClick;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(scale, duration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(1, duration / 2).OnComplete(() => onClick.Invoke());
    }
}