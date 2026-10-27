using DG.Tweening;
using DingDing.Core;
using DingDing.Core.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Runs the break between rounds: deals three cards, counts down, and sends
// the chosen one to the engine.
public class CornerScreen : MonoBehaviour
{
    public GameObject root;                 // the whole corner panel
    public CanvasGroup contentGroup;        // hidden until the board is gone
    public RectTransform boardRoot;         // slides out of the way
    public BoardView boardView;
    public EventPlayer eventPlayer;
    public HudView hud;
    public TimerRing timerRing;

    public CardView[] cardViews;            // three, left to right
    public CardDisplay[] cardDisplays;      // eight, one asset per card
    public Sprite attackBackSprite;
    public Sprite tacticBackSprite;

    public float totalSeconds = 60f;
    public float boardSlideDuration = 0.35f;
    public float boardHideDistance = 1400f;
    public float fadeInDuration = 0.25f;
    public float dealDuration = 0.25f;
    public float dealGap = 0.08f;
    public float flipStagger = 0.09f;
    public float discardFadeDuration = 0.25f;
    public float flyToSlotDuration = 0.4f;

    private Match match;
    private System.Random uiRandom = new System.Random();
    private float secondsLeft;
    private bool timerRunning;
    private int lastWholeSecond;
    private bool isClosing;
    private bool cardsRevealed;
    private Vector2 boardRestPosition;

    private GameSounds Sounds { get { return AudioManager.Instance.sounds; } }

    public void Open(Match openMatch)
    {
        match = openMatch;
        isClosing = false;
        cardsRevealed = false;

        // The round that just ended.
        AudioManager.Instance.Play(Sounds.bellLong);

        // The panel has to be awake to run coroutines, so it is the content
        // that stays invisible rather than the object itself.
        contentGroup.alpha = 0f;
        contentGroup.blocksRaycasts = true;
        root.SetActive(true);

        StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        // Where the board sits while it is being played, so it can be put
        // back exactly there afterwards.
        boardRestPosition = boardRoot.anchoredPosition;

        float hiddenY = boardRestPosition.y - boardHideDistance;

        yield return boardRoot.DOAnchorPosY(hiddenY, boardSlideDuration).SetEase(Ease.InQuad).WaitForCompletion();
        yield return contentGroup.DOFade(1f, fadeInDuration).WaitForCompletion();

        List<CardOffer> offers = CardRules.OfferCards(match);
        yield return DealCards(offers).WaitForCompletion();

        secondsLeft = totalSeconds;
        lastWholeSecond = Mathf.CeilToInt(secondsLeft);
        timerRing.Show(secondsLeft, totalSeconds);
        timerRunning = true;
    }

    private Sequence DealCards(List<CardOffer> offers)
    {
        Sequence sequence = DOTween.Sequence();

        for (int index = 0; index < cardViews.Length; index++)
        {
            CardOffer offer = offers[index];
            CardDisplay display = DisplayFor(offer.Card.Id);

            Sprite back = offer.Card.CardClass == CardClass.Attack ? attackBackSprite : tacticBackSprite;

            Sprite art = display.art;
            if (offer.Card.NeedsColor) art = boardView.SpriteOf(offer.Color);

            cardViews[index].Show(this, offer, back, art, display.displayName, EffectTextOf(display, offer));

            cardViews[index].gameObject.SetActive(true);
            cardViews[index].rectTransform.localScale = Vector3.zero;

            sequence.Insert(index * dealGap, cardViews[index].rectTransform.DOScale(1f, dealDuration).SetEase(Ease.OutBack));
        }

        return sequence;
    }

    private CardDisplay DisplayFor(CardId id)
    {
        foreach (var display in cardDisplays)
        {
            if (display != null && display.id == id) return display;
        }

        return cardDisplays[0];
    }

    // Read the Opponent names a color, and the name is printed in it.
    private string EffectTextOf(CardDisplay display, CardOffer offer)
    {
        if (!offer.Card.NeedsColor) return display.effectText;

        string colorName = NameOf(offer.Color);
        Color color = boardView.ColorOf(offer.Color);
        string markup = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + colorName + "</color>";

        return display.effectText.Replace("{COLOR}", markup);
    }

    private static string NameOf(PieceColor color)
    {
        if (color == PieceColor.Red) return "RED";
        if (color == PieceColor.Blue) return "BLUE";
        if (color == PieceColor.Orange) return "ORANGE";
        if (color == PieceColor.Green) return "GREEN";

        return "PURPLE";
    }

    private void Update()
    {
        if (!timerRunning) return;

        secondsLeft = secondsLeft - Time.deltaTime;
        if (secondsLeft < 0f) secondsLeft = 0f;

        timerRing.Show(secondsLeft, totalSeconds);

        int wholeSeconds = Mathf.CeilToInt(secondsLeft);
        if (wholeSeconds != lastWholeSecond)
        {
            lastWholeSecond = wholeSeconds;

            if (wholeSeconds <= timerRing.warningSeconds && wholeSeconds > 0)
            {
                AudioManager.Instance.Play(Sounds.movesWarning);
                timerRing.Pulse();
            }
        }

        if (secondsLeft <= 0f)
        {
            timerRunning = false;
            StartCoroutine(ChooseForPlayer());
        }
    }

    // All three turn over together; a single card held back would only make
    // the player tap three times for the same information.
    private Sequence FlipAll()
    {
        cardsRevealed = true;

        AudioManager.Instance.Play(Sounds.cardFlip);

        Sequence sequence = DOTween.Sequence();

        for (int index = 0; index < cardViews.Length; index++)
        {
            sequence.Join(cardViews[index].Flip(index * flipStagger));
        }

        return sequence;
    }

    private IEnumerator ChooseForPlayer()
    {
        if (!cardsRevealed)
        {
            yield return FlipAll().WaitForCompletion();
            yield return new WaitForSeconds(0.6f);
        }

        CardView chosen = cardViews[uiRandom.Next(0, cardViews.Length)];
        yield return Confirm(chosen);
    }

    public void OnCardTapped(CardView cardView)
    {
        if (isClosing) return;

        if (!cardsRevealed)
        {
            FlipAll();
            return;
        }

        timerRunning = false;
        timerRing.Stop();
        StartCoroutine(Confirm(cardView));
    }

    private IEnumerator Confirm(CardView chosen)
    {
        isClosing = true;
        timerRunning = false;
        timerRing.Stop();

        AudioManager.Instance.Play(Sounds.cardSelect);

        // Called before the flight, because the slot it flies to is the one
        // this card is about to fill.
        List<object> events = MatchFlow.SelectCornerCard(match, chosen.Offer);

        // The counters belong to the new round from here on; the card slot
        // waits until the card has landed in it.
        hud.Refresh(false);

        foreach (var cardView in cardViews)
        {
            if (cardView != chosen) cardView.FadeOut(discardFadeDuration);
        }

        RectTransform slot = hud.SlotTransformFor(match.ChosenCards.Count - 1);

        Sequence flight = DOTween.Sequence();
        flight.Join(chosen.rectTransform.DOMove(slot.position, flyToSlotDuration).SetEase(Ease.InOutQuad));
        flight.Join(chosen.rectTransform.DOScale(0.15f, flyToSlotDuration).SetEase(Ease.InQuad));

        yield return flight.WaitForCompletion();

        AudioManager.Instance.Play(Sounds.cardActivate);

        hud.Refresh(true);

        foreach (var cardView in cardViews)
        {
            cardView.gameObject.SetActive(false);
        }

        // The panel is only faded out here; switching it off would stop this
        // coroutine, which lives on it.
        contentGroup.alpha = 0f;
        contentGroup.blocksRaycasts = false;

        yield return boardRoot.DOAnchorPosY(boardRestPosition.y, boardSlideDuration).SetEase(Ease.OutQuad).WaitForCompletion();

        // The new round opens.
        AudioManager.Instance.Play(Sounds.bell);

        // Early Attack and Coach's Advice drop a special piece on the board.
        yield return eventPlayer.PlayEvents(events);
        hud.Refresh();

        root.SetActive(false);
    }
}