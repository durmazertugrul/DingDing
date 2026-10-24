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
    private Tween wobbleTween;
    private Tween idleTween;
    public RectTransform RectTransform { get; private set; }
    public Image glowImage;
    public Color specialGlowColor = new Color(0.96f, 0.73f, 0.23f, 0.75f);
    public float swayDuration = 0.55f;
    public float swayAngle = 6f;
    public float glowPulseDuration = 0.8f;
    private Tween glowTween;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public void SetPiece(Piece piece, Sprite sprite)
    {
        Piece = piece;
        image.sprite = sprite;
        StartIdle(piece);
    }

    private void StartIdle(Piece piece)
    {
        if (idleTween != null && idleTween.IsActive()) idleTween.Kill();
        if (glowTween != null && glowTween.IsActive()) glowTween.Kill();

        RectTransform.localRotation = Quaternion.identity;

        if (!piece.IsSpecial())
        {
            glowImage.gameObject.SetActive(false);
            return;
        }

        RectTransform.localRotation = Quaternion.Euler(0f, 0f, -swayAngle);
        idleTween = RectTransform.DOLocalRotate(new Vector3(0f, 0f, swayAngle), swayDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        glowImage.gameObject.SetActive(true);
        glowImage.rectTransform.localScale = Vector3.one;
        glowImage.color = new Color(specialGlowColor.r, specialGlowColor.g, specialGlowColor.b, specialGlowColor.a * 0.45f);

        Sequence glowSequence = DOTween.Sequence();
        glowSequence.Join(glowImage.rectTransform.DOScale(1.08f, glowPulseDuration).SetEase(Ease.InOutSine));
        glowSequence.Join(glowImage.DOFade(specialGlowColor.a, glowPulseDuration).SetEase(Ease.InOutSine));
        glowSequence.SetLoops(-1, LoopType.Yoyo);

        glowTween = glowSequence;
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

        if (idleTween != null && idleTween.IsActive()) idleTween.Kill();
        if (glowTween != null && glowTween.IsActive()) glowTween.Kill();

        idleTween = null;
        glowTween = null;
        glowImage.gameObject.SetActive(false);
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
    public Tween Wobble(AnimationSettings settings)
    {
        // A punch that is still running would stack and leave the tile tilted.
        if (wobbleTween != null && wobbleTween.IsActive())
            wobbleTween.Kill();

        RectTransform.localRotation = Quaternion.identity;

        wobbleTween = RectTransform.DOPunchRotation(new Vector3(0f, 0f, 8f), settings.invalidSwapDuration).OnComplete(() => RectTransform.localRotation = Quaternion.identity);

        return wobbleTween;
    }

    // Played when a falling tile lands.
    public Tween LandSquash(AnimationSettings settings)
    {
        return RectTransform.DOPunchScale(new Vector3(0.12f, -0.12f, 0f), settings.landSquashDuration);
    }
        

}