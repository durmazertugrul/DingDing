using DG.Tweening;
using DingDing.Core.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// One card in the corner. Starts face down; the first tap turns all three
// over, the next one picks this card.
public class CardView : MonoBehaviour, IPointerClickHandler
{
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public Image backImage;
    public GameObject faceRoot;
    public Image artImage;
    public TMP_Text titleText;
    public TMP_Text effectText;

    public float flipHalfDuration = 0.22f;
    public float flipHopHeight = 60f;
    public float flipRiseScale = 1.07f;

    public CardOffer Offer { get; private set; }
    public bool IsFaceUp { get; private set; }

    private CornerScreen cornerScreen;

    public void Show(CornerScreen screen, CardOffer offer, Sprite back, Sprite art, string title, string effect)
    {
        cornerScreen = screen;
        Offer = offer;
        IsFaceUp = false;

        backImage.sprite = back;
        artImage.sprite = art;
        titleText.text = title;
        effectText.text = effect;

        backImage.gameObject.SetActive(true);
        faceRoot.SetActive(false);

        canvasGroup.alpha = 1f;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }

    // The card lifts, turns on its own axis, and settles back down. The face
    // is swapped at the halfway point, where the card is edge on and the
    // swap cannot be seen.
    public Sequence Flip(float delay)
    {
        IsFaceUp = true;

        float restY = rectTransform.anchoredPosition.y;
        float half = flipHalfDuration;
        float start = delay;

        Sequence sequence = DOTween.Sequence();

        sequence.Insert(start, rectTransform.DOLocalRotate(new Vector3(0f, 90f, 0f), half).SetEase(Ease.InSine));
        sequence.Insert(start, rectTransform.DOAnchorPosY(restY + flipHopHeight, half).SetEase(Ease.OutSine));
        sequence.Insert(start, rectTransform.DOScale(flipRiseScale, half).SetEase(Ease.OutSine));

        sequence.InsertCallback(start + half, TurnOver);

        sequence.Insert(start + half, rectTransform.DOLocalRotate(Vector3.zero, half).SetEase(Ease.OutSine));
        sequence.Insert(start + half, rectTransform.DOAnchorPosY(restY, half).SetEase(Ease.InSine));
        sequence.Insert(start + half, rectTransform.DOScale(1f, half).SetEase(Ease.InSine));

        return sequence;
    }

    // Edge on, so the card carries on turning in the same direction instead
    // of winding back.
    private void TurnOver()
    {
        rectTransform.localRotation = Quaternion.Euler(0f, -90f, 0f);

        backImage.gameObject.SetActive(false);
        faceRoot.SetActive(true);
    }

    // The cards that were not picked just go out where they stand.
    public Tween FadeOut(float duration)
    {
        return canvasGroup.DOFade(0f, duration).SetEase(Ease.InQuad);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cornerScreen.OnCardTapped(this);
    }
}