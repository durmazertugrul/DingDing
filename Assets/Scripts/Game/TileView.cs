using DG.Tweening;
using DingDing.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileView : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // Represents a single piece visually, tracks its cell and associated Piece,
    // updates its sprite, and returns to the pool instead of being destroyed.

    public Image image;
    public BoardInput boardInput;
    public Cell Cell { get; set; }
    public Piece Piece { get; private set; }

    public RectTransform RectTransform { get; private set; }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public void SetPiece(Piece piece, Sprite sprite)
    {
        Piece = piece;
        image.sprite = sprite;
    }

    public void SetPosition(Vector2 anchoredPosition)
    {
        RectTransform.anchoredPosition = anchoredPosition;
    }

    // The tweens are only built here; EventPlayer decides when to wait for them.
    public Tween MoveTo(Vector2 targetPosition, float duration, Ease ease)
    {
        return RectTransform.DOAnchorPos(targetPosition, duration).SetEase(ease);
    }

    public Sequence Pop(AnimationSettings settings)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(RectTransform.DOScale(1.18f, settings.popGrowDuration).SetEase(settings.popGrowEase));
        sequence.Append(RectTransform.DOScale(0f, settings.popShrinkDuration).SetEase(settings.popShrinkEase));

        return sequence;
    }

    public Tween GrowFromZero(AnimationSettings settings)
    {
        RectTransform.localScale = Vector3.zero;
        return RectTransform.DOScale(1f, settings.specialGrowDuration).SetEase(settings.specialGrowEase);
    }

    // A pop tween leaves the scale at zero, so the pool resets it on the way back.
    public void ResetVisualState()
    {
        RectTransform.DOKill();
        RectTransform.localScale = Vector3.one;
        RectTransform.localRotation = Quaternion.identity;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        boardInput.OnTilePressed(this, eventData);
    }
        

    public void OnDrag(PointerEventData eventData)
    {
        boardInput.OnTileDragged(eventData);
    }
        

    public void OnPointerUp(PointerEventData eventData)
    {
        boardInput.OnTileReleased(eventData);
    }

    // Played when a tapped piece is not special.
    public Tween Wobble(AnimationSettings settings)
    {
        return RectTransform.DOPunchRotation(new Vector3(0, 0, 8f), settings.invalidSwapDuration);
    }
        

    // Played when a swap is rejected.
    public Tween ShakeInvalid(AnimationSettings settings)
    {
        return RectTransform.DOShakePosition(settings.invalidSwapDuration, new Vector3(10f, 0, 0));

    }

}