using DG.Tweening;
using DingDing.Core;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Fills in the score card. Two separate cards exist in the scene, one for a
// knockout and one for a loss on points; only one of them is ever shown.
// The card builds itself up a line at a time rather than appearing whole.
public class ResultScreen : MonoBehaviour
{
    // One line of a card: the damage that round did, and how it ended.
    [System.Serializable]
    public class RoundRow
    {
        public TMP_Text damageText;
        public Image markerImage;
    }

    public GameObject root;
    public GameObject knockoutScreen;
    public GameObject lossScreen;

    public TMP_Text subtitleText;           // ROUND 2, MOVE 6
    public RoundRow[] knockoutRows;         // three, round one first
    public TMP_Text movesLeftText;          // the number, not the label
    public TMP_Text earlyKnockoutText;      // the number, not the label
    public TMP_Text knockoutScoreText;

    public RoundRow[] lossRows;             // three, round one first
    public TMP_Text lossScoreText;

    public Sprite wonMarker;
    public Sprite lostMarker;
    public Sprite knockoutMarker;
    public Sprite notPlayedMarker;

    public float damageCountDuration = 0.5f;
    public float bonusCountDuration = 0.4f;
    public float totalCountDuration = 1f;
    public float markerPopDuration = 0.25f;
    public float stepGap = 0.15f;

    public void Show(Match match)
    {
        bool isKnockout = match.Outcome == MatchOutcome.Knockout;

        root.SetActive(true);
        knockoutScreen.SetActive(isKnockout);
        lossScreen.SetActive(!isKnockout);

        if (isKnockout)
        {
            subtitleText.text = "ROUND " + match.CurrentRound + ", MOVE " + match.KnockoutMove;

            // The rows stay on screen the whole time; only their numbers move.
            movesLeftText.text = "0";
            earlyKnockoutText.text = "0";
        }

        RoundRow[] rows = isKnockout ? knockoutRows : lossRows;

        // Everything starts empty and grows in, one piece at a time.
        foreach (var row in rows)
        {
            row.damageText.text = "0";
            row.markerImage.rectTransform.localScale = Vector3.zero;
        }

        TMP_Text scoreText = isKnockout ? knockoutScoreText : lossScoreText;
        scoreText.text = "0";
        scoreText.rectTransform.localScale = Vector3.one;

        StartCoroutine(Reveal(match, rows, scoreText, isKnockout));
    }

    private IEnumerator Reveal(Match match, RoundRow[] rows, TMP_Text scoreText, bool isKnockout)
    {
        for (int round = 1; round <= GameRules.RoundCount; round++)
        {
            RoundRow row = rows[round - 1];
            RoundResult result = match.RoundResults[round];

            if (result == RoundResult.NotPlayed)
                row.damageText.text = "-";
            else
                yield return CountUp(row.damageText, match.RoundDamage[round], damageCountDuration).WaitForCompletion();

            row.markerImage.sprite = MarkerFor(result);
            yield return PopIn(row.markerImage.rectTransform).WaitForCompletion();
            yield return WaitFor(stepGap);
        }

        if (isKnockout)
        {
            int leftoverPoints = match.MovesLeftInRound * GameRules.PointsPerLeftoverMove;
            int roundsNotNeeded = GameRules.RoundCount - match.CurrentRound;
            int earlyPoints = roundsNotNeeded * GameRules.EarlyKnockoutBonusPerRound;

            yield return CountUp(movesLeftText, leftoverPoints, bonusCountDuration).WaitForCompletion();
            yield return WaitFor(stepGap);

            yield return CountUp(earlyKnockoutText, earlyPoints, bonusCountDuration).WaitForCompletion();
            yield return WaitFor(stepGap);
        }

        yield return CountUp(scoreText, match.Score, totalCountDuration).WaitForCompletion();

        scoreText.rectTransform.DOPunchScale(Vector3.one * 0.25f, 0.35f, 1, 0.5f).SetUpdate(true);
    }

    private Sprite MarkerFor(RoundResult result)
    {
        if (result == RoundResult.Won) return wonMarker;
        if (result == RoundResult.Lost) return lostMarker;
        if (result == RoundResult.Knockout) return knockoutMarker;

        return notPlayedMarker;
    }

    // Numbers run up the same way the score does during a match.
    private Tween CountUp(TMP_Text target, int finalValue, float duration)
    {
        int shownValue = 0;

        return DOTween.To(() => shownValue, value => { shownValue = value; target.text = value.ToString("N0", CultureInfo.InvariantCulture); }, finalValue, duration).SetEase(Ease.OutCubic).SetUpdate(true);
    }

    private Tween PopIn(RectTransform target)
    {
        target.localScale = Vector3.zero;

        return target.DOScale(1f, markerPopDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    // Time.timeScale may be zero here, so the wait has to ignore it.
    private IEnumerator WaitFor(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
    }
}