using DG.Tweening;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public RectTransform target; //the board 
    private Vector2 restPosition;

    private void Awake()
    {
        restPosition = target.anchoredPosition;
    }

    public void Shake(float duration, float strength)
    {
        target.DOKill();
        target.anchoredPosition = restPosition;
        target.DOShakeAnchorPos(duration, strength, 20, 90f).OnComplete(() => target.anchoredPosition = restPosition);
    }



}
