using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class FxPool : MonoBehaviour
{
    public FxView fxPrefab;
    public RectTransform fxParent;
    private Queue<FxView> pool = new Queue<FxView>();

    private FxView Take(Sprite sprite, Color color, Vector2 position, float size)
    {
        FxView fxView;

        if (pool.Count > 0)
            fxView = pool.Dequeue();
        else
            fxView = Instantiate(fxPrefab, fxParent, false);

        fxView.gameObject.SetActive(true);
        fxView.Setup(sprite, color, position, size);
        return fxView;
    }

    private void Return(FxView fxView)
    {
        fxView.ResetVisualState();
        fxView.gameObject.SetActive(false);
        pool.Enqueue(fxView);
    }

    // Grows in place and fades out, then goes back to the pool on its own.
    public void PlayBurst(Sprite sprite, Color color, Vector2 position,float size, float duration, float endScale)
    {
        FxView fxView = Take(sprite, color, position, size);

        Sequence sequence = DOTween.Sequence();

        sequence.Join(fxView.RectTransform.DOScale(endScale, duration).SetEase(Ease.OutQuad));
        sequence.Join(fxView.image.DOFade(0f, duration));
        sequence.OnComplete(() => Return(fxView));
    }

    // Flies out from the centre while shrinking and fading.
    public void PlayFlyOut(Sprite sprite, Color color, Vector2 position,Vector2 offset, float size, float duration)
    {
        FxView fxView = Take(sprite, color, position, size);

        Sequence sequence = DOTween.Sequence();
        sequence.Join(fxView.RectTransform.DOAnchorPos(position + offset, duration).SetEase(Ease.OutQuad));
        sequence.Join(fxView.RectTransform.DOScale(0.2f, duration));
        sequence.Join(fxView.image.DOFade(0f, duration));
        sequence.OnComplete(() => Return(fxView));
    }

    // Travels along a row or a column and leaves the board.
    public void PlayTrail(Sprite sprite, Color color, Vector2 position,Vector2 endPosition, float size, float duration)
    {
        FxView fxView = Take(sprite, color, position, size);

        Sequence sequence = DOTween.Sequence();
        sequence.Join(fxView.RectTransform.DOAnchorPos(endPosition, duration).SetEase(Ease.OutQuad));
        sequence.Join(fxView.image.DOFade(0f, duration));
        sequence.OnComplete(() => Return(fxView));
    }





}
