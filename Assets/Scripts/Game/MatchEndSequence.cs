using DingDing.Core;
using System.Collections;
using UnityEngine;

// The beat between the last move and the score card.
public class MatchEndSequence : MonoBehaviour
{
    public ResultScreen resultScreen;
    public ScreenShake screenShake;
    public HudView hud;
    public GameObject gameRoot;         // the HUD and the board together

    public float knockoutHold = 1.4f;
    public float lossHold = 0.8f;
    public float knockoutShakeDuration = 0.5f;
    public float knockoutShakeStrength = 30f;

    public void Begin(Match match)
    {
        StartCoroutine(Run(match));
    }

    private IEnumerator Run(Match match)
    {
        if (match.Outcome == MatchOutcome.Knockout)
        {
            screenShake.Shake(knockoutShakeDuration, knockoutShakeStrength);
            hud.ApplyDamage(0, 0);          // the dazed face, nothing left to take
            yield return new WaitForSeconds(knockoutHold);
        }
        else
        {
            // The board is left alone for a moment so the last move reads.
            yield return new WaitForSeconds(lossHold);
        }

        gameRoot.SetActive(false);
        resultScreen.Show(match);
    }
}