using DingDing.Core;
using DingDing.Core.Cards;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Shows what the match holds. Reads from Match, never writes to it.
public class HudView : MonoBehaviour
{
    public LevelBoxers levelBoxers;
    public OpponentFace opponentFace;
    public HealthBar healthBar;

    public TMP_Text movesText;
    public TMP_Text scoreText;

    public Image[] roundDots;           // three, left to right
    public Sprite activeDotSprite;      // the round being played
    public Sprite passedDotSprite;      // rounds already over
    public Sprite futureDotSprite;      // rounds not reached yet

    public Image[] cardSlots;           // two mini cards, left to right
    public Sprite attackBackSprite;
    public Sprite tacticBackSprite;

    public int lowMoveWarning = 5;
    public Color lowMovesColor;

    public float scoreCountDuration = 0.4f;
    public float scorePunchScale = 0.25f;

    private Match match;
    private Tween movesPulse;

    private Color normalMovesColor;
    private int displayedScore;
    private Tween scoreTween;

    private void Awake()
    {
        // The colour the text was authored with; the warning colour is the
        // only one worth keeping in the Inspector.
        normalMovesColor = movesText.color;
    }

    public void Bind(Match boundMatch)
    {
        match = boundMatch;

        opponentFace.SetBoxer(levelBoxers.BoxerFor(match.Level.Number));
        opponentFace.Refresh(match.OpponentHealth, match.Level.StartingHealth,false);
        healthBar.SetInstantly(match.OpponentHealth, match.Level.StartingHealth);

        displayedScore = match.Score;
        scoreText.text = displayedScore.ToString("N0");

        Refresh();
    }

    // Called once the animations of a move have finished.
    public void Refresh()
    {
        RefreshRoundDots();
        RefreshMoves();
        RefreshScore();
        RefreshCardSlots();
    }

    // Called for every hit, while the move is still playing, so the bar
    // moves with the pieces instead of after them.
    public void ApplyDamage(int healthAfter)
    {
        int maxHealth = match.Level.StartingHealth;
        bool isKnockout = healthAfter == 0;

        healthBar.ApplyDamage(healthAfter, maxHealth);
        opponentFace.Refresh(healthAfter, maxHealth, isKnockout);
    }

    private void RefreshRoundDots()
    {
        for (int index = 0; index < roundDots.Length; index++)
        {
            int roundNumber = index + 1;

            if (roundNumber < match.CurrentRound)
                roundDots[index].sprite = passedDotSprite;
            else if (roundNumber == match.CurrentRound)
                roundDots[index].sprite = activeDotSprite;
            else
                roundDots[index].sprite = futureDotSprite;
        }
    }

    private void RefreshMoves()
    {
        movesText.text = match.MovesLeftInRound.ToString();

        // An endless pulse has to be stopped, or the text stays enlarged
        // once the count climbs back above the warning line.
        if (movesPulse != null && movesPulse.IsActive())
            movesPulse.Kill();

        movesText.rectTransform.localScale = Vector3.one;

        if (match.MovesLeftInRound <= lowMoveWarning)
        {
            movesText.color = lowMovesColor;
            movesPulse = movesText.rectTransform.DOScale(1.15f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        }
        else
        {
            movesText.color = normalMovesColor;
        }
    }

    private void RefreshScore()
    {
        if (displayedScore == match.Score)
            return;

        if (scoreTween != null && scoreTween.IsActive())
            scoreTween.Kill();

        // The number runs up to the new total instead of jumping, and the
        // box takes a knock so the gain is felt.
        scoreTween = DOTween.To(
                () => displayedScore,
                value =>
                {
                    displayedScore = value;
                    scoreText.text = value.ToString("N0");
                },
                match.Score,
                scoreCountDuration).SetEase(Ease.OutCubic);

        scoreText.rectTransform.DOKill();
        scoreText.rectTransform.localScale = Vector3.one;
        scoreText.rectTransform.DOPunchScale(Vector3.one * scorePunchScale, scoreCountDuration, 1, 0.5f);
    }

    private void RefreshCardSlots()
    {
        for (int index = 0; index < cardSlots.Length; index++)
        {
            // The dashed empty box underneath shows through when the mini
            // card is switched off.
            if (index >= match.ChosenCards.Count)
            {
                cardSlots[index].gameObject.SetActive(false);
                continue;
            }

            CardInfo card = match.ChosenCards[index];

            cardSlots[index].sprite = card.CardClass == CardClass.Attack? attackBackSprite: tacticBackSprite;
            cardSlots[index].gameObject.SetActive(true);
        }
    }
}